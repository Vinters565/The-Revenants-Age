using System;
using System.Linq;
using Interface.Menu.ForConsole;
using TheRevenantsAge;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraControlActions cameraActions;
    private InputAction movement;
    private Transform cameraTransform;
    private new Camera camera;

    [SerializeField] private GameObject map;
    private Vector3 mapPointMax;
    private Vector3 mapPointMin;

    [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float minimapSpeed = 10f;
    private float speed;

    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float damping = 15f;

    [SerializeField] private float zoomStepSize = 2f;
    [SerializeField] private float zoomDampening = 7.5f;
    [SerializeField] private float zoomMinHeight = 5f;
    [SerializeField] private float zoomMaxHeight = 50f;
    [SerializeField] [Range(0f, 0.1f)] private float edgeTolerance = 0.05f;

    private static CameraController instance;

    public static CameraController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(CameraController)) as CameraController;
                if (instance == null)
                {
                    instance = new CameraController();
                }
            }

            return instance;
        }
    }

    public bool IsActive { get; set; }

    private Vector3 targetPosition;

    public float zoomHeight;

    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;

    private bool destinationReached;
    private Vector3 destinationPoint;

    public Vector3 DestinationPoint
    {
        set
        {
            destinationPoint = value;
            destinationReached = false;
        }
    }

    private void Awake()
    { 
        cameraActions = new CameraControlActions();
        var objCamera = GameObject.FindGameObjectWithTag("MainCamera");
        camera = objCamera.GetComponent<Camera>();
        cameraTransform = objCamera.transform;
        mapPointMax = map.GetComponent<Collider>().bounds.max;
        mapPointMin = map.GetComponent<Collider>().bounds.min;
        transform.position = destinationPoint = TheRevenantsAge.GlobalMapController.Groups.First().transform.position;
    }

    private void OnEnable()
    {
        zoomHeight = cameraTransform.localPosition.y;
        cameraTransform.LookAt(transform);

        lastPosition = transform.position;

        movement = cameraActions.Camera.Move;
        cameraActions.Camera.ZoomCamera.performed += ZoomCamera;
        MinimapController.Instance.MoveToDestinationEvent += OnMoveToDestination;
        cameraActions.Camera.Enable();
    }
    
    private void OnDisable()
    {
        cameraActions.Camera.ZoomCamera.performed -= ZoomCamera;
        MinimapController.Instance.MoveToDestinationEvent -= OnMoveToDestination;
        cameraActions.Camera.Disable();
    }

    private void Update()
    {
        if (!IsActive) return;
        if (destinationReached)
        {
            GetKeyboardMovement();
            //CheckMouseAtScreenEdge();

            UpdateVelocity();
            UpdateBasePosition();
            UpdateCameraPosition();
        }
        else
            OnMoveToDestination(destinationPoint);


        transform.position = CheckPosition(transform.position, transform.position);
    }

    private void GetKeyboardMovement()
    {
        var receivedValue = movement.ReadValue<Vector2>();
        var inputValue = receivedValue.x * GetCameraRight()
                         + receivedValue.y * GetCameraUp();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
            targetPosition += inputValue;
    }

    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            var position = transform.position;
            position += targetPosition * (speed * Time.deltaTime);
            transform.position = position;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }

        targetPosition = Vector3.zero;
    }

    private void UpdateVelocity()
    {
        var position = transform.position;
        horizontalVelocity = (position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = position;
    }

    private Vector3 GetCameraUp()
    {
        var up = cameraTransform.up;
        up.y = 0f;
        return up;
    }

    private Vector3 GetCameraRight()
    {
        var right = cameraTransform.right;
        right.y = 0f;
        return right;
    }

    private void ZoomCamera(InputAction.CallbackContext obj)
    {
        float inputValue = -obj.ReadValue<Vector2>().y / 100f;

        if (Mathf.Abs(inputValue) > 0.1f)
        {
            zoomHeight = cameraTransform.localPosition.y + inputValue * zoomStepSize;

            if (zoomHeight < zoomMinHeight)
                zoomHeight = zoomMinHeight;
            else if (zoomHeight > zoomMaxHeight)
                zoomHeight = zoomMaxHeight;
        }
    }

    private void UpdateCameraPosition()
    {
        var localPosition = cameraTransform.localPosition;
        var zoomTarget = new Vector3(localPosition.x, zoomHeight, localPosition.z);

        localPosition = Vector3.Lerp(localPosition, zoomTarget, Time.deltaTime * zoomDampening);
        cameraTransform.localPosition = localPosition;
    }

    private void CheckMouseAtScreenEdge()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var moveDirection = Vector3.zero;

        if (mousePosition.x < edgeTolerance * Screen.width)
            moveDirection += -GetCameraRight();
        else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
            moveDirection += GetCameraRight();

        if (mousePosition.y < edgeTolerance * Screen.height)
            moveDirection += -GetCameraUp();
        else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
            moveDirection += GetCameraUp();

        targetPosition += moveDirection;
    }

    void OnMoveToDestination(Vector3 point)
    {
        destinationReached = false;
        destinationPoint = CheckPosition(transform.position, point);
        transform.position = Vector3.Lerp(transform.position, destinationPoint, minimapSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, destinationPoint) <= 1f)
        {
            destinationReached = true;
            lastPosition = transform.position;
        }
    }

    private Vector3 CheckPosition(Vector3 fromPoint, Vector3 toPoint)
    {
        var minPointTo = GetCameraFrustumPoint(new Vector3(0f, 0f));
        var maxPointTo = GetCameraFrustumPoint(new Vector3(Screen.width, Screen.height));
        var resultPoint = new Vector3(
            Math.Clamp(toPoint.x, mapPointMin.x + 5 + (maxPointTo.x - fromPoint.x),
                mapPointMax.x - 5 - (fromPoint.x - minPointTo.x)),
            0,
            Math.Clamp(toPoint.z, mapPointMin.z + 5 + (maxPointTo.z - fromPoint.z),
                mapPointMax.z - 5 - (fromPoint.z - minPointTo.z)));
        return resultPoint;
    }

    private Vector3 GetCameraFrustumPoint(Vector3 position)
    {
        var positionRay = camera.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(positionRay, out hit, Mathf.Infinity))
            return hit.point;
        return new Vector3();
    }

    public void MoveCamera(Vector3 point)
    {
        destinationReached = false;
        transform.position = destinationPoint = point;
    }
    
    private void GameConsoleOnActivatedOrDeactivated(bool isActive)
    {
        IsActive = !isActive;
    }
}