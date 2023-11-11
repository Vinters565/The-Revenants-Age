using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    private const float MOVE_SPEED = 10f;
    private const float LIFT_SPEED = 20f;
    
    private const float MOVE_LERP_SMOOTH = 5f;
    private const float ROTATE_LERP_SMOOTH = 5f;
    
    private const float GOAL_POSITION_FINISH_DISTANCE = 0.05f;
    private const float GOAL_ROTATION_FINISH_ANGLE = 0.005f;


    private const float X_CENTER_RATE = 0.4f; 
    private const float Y_CENTER_RATE = 0.4f; 
    private int LEFT_X_CENTER_BORDER;
    private int RIGHT_X_CENTER_BORDER;
    private int TOP_Y_CENTER_BORDER;
    private int BOTTOM_Y_CENTER_BORDER;

    public static CameraScript Instance;

    [SerializeField] private float isometricCameraSensivity = 150f;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] [Min(0)] private float minCameraHeight;
    [SerializeField] [Min(0)] private float maxCameraHeight;
    [SerializeField] private float fPCameraSensivity = 100f;
    [SerializeField] private EventSystem eventSystem;

    private Camera mainCamera;
    private bool rightMouseButtonPressed = false;
    private Vector3 oldIsometricLocalPosition;
    private Vector3 oldIsometricPosition;
    //private Vector3 oldIsometricRotation;
    private Vector3 oldIsometricLocalRotate;
    
    private float oldRotationX;
    private float oldRotationY;
    
    private Vector3? goalPosition;
    private float? goalXRotation;
    private float? goalYRotation;

    private Transform observeTransform;
    private Vector3 observeOffset;

    public float RotationX { get; set; }
    public float RotationY { get; set; }
    public FightCameraMode Mode { get; set; } = FightCameraMode.Isometric;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        goalPosition = transform.position;
        mainCamera = GetComponentInChildren<Camera>();
        
        LEFT_X_CENTER_BORDER = (int)(mainCamera.pixelWidth * X_CENTER_RATE);
        RIGHT_X_CENTER_BORDER = mainCamera.pixelWidth - LEFT_X_CENTER_BORDER;
        TOP_Y_CENTER_BORDER = (int)(mainCamera.pixelHeight * Y_CENTER_RATE);
        BOTTOM_Y_CENTER_BORDER = mainCamera.pixelHeight - TOP_Y_CENTER_BORDER;
        
        var body = GetComponent<Rigidbody>();
        RotationX = mainCamera.transform.localEulerAngles.x;
        RotationY = mainCamera.transform.parent.localEulerAngles.y;
        body.freezeRotation = true;
    }

    private void Update()
    {
        if (CameraReachedGoal())
        {
            if (Mode == FightCameraMode.Isometric)
            {
                KeyBoardMove();
                MouseRotate();
                MouseWheelLift();
            }
            else if(Mode == FightCameraMode.FPShooting)
            {
                AimMouseRotate();
            }
        }
        KeyboardInputHandling();
    }

    private void KeyboardInputHandling()
    {
        if (Mode == FightCameraMode.FPShooting)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StateController.Instance.SwitchShootPhase();
                //ChangeToIsometricMode(true);
            }
        }
    }

    private void FixedUpdate()
    {
        GoalPositionLerp();
        GoalXRotationLerp();
        GoalYRotationLerp();
        ObserveToTarget();
    }

    private void GoalPositionLerp()
    {
        if (goalPosition.HasValue)
        {
            if (Vector3.Distance(goalPosition.Value, transform.position) > GOAL_POSITION_FINISH_DISTANCE)
            {
                transform.position = Vector3.Lerp(transform.position, goalPosition.Value, Time.deltaTime * MOVE_LERP_SMOOTH);
            }
            else
            {
                goalPosition = null;
            }
        }
    }

    private void GoalXRotationLerp()
    {
        if (goalXRotation.HasValue)
        {
            var currentX = mainCamera.transform.localEulerAngles.x;
            if (Math.Abs(currentX - goalXRotation.Value) > GOAL_ROTATION_FINISH_ANGLE)
            {
                mainCamera.transform.localEulerAngles = Vector3.Lerp(mainCamera.transform.localEulerAngles,
                    new Vector3(goalXRotation.Value, 0, 0), Time.deltaTime * ROTATE_LERP_SMOOTH);
            }
            else
            {
                RotationX = currentX;
                goalXRotation = null;
            }
        }
    }

    private void GoalYRotationLerp()
    {
        if (goalYRotation.HasValue)
        {
            var currentY = transform.localEulerAngles.y;
            var rotateOffset = Math.Abs(currentY - goalYRotation.Value);
            if (rotateOffset > 180f)
                goalYRotation = GetMinRotationOffsetTarget(currentY, goalYRotation.Value);
            
            if (rotateOffset > GOAL_ROTATION_FINISH_ANGLE)
            {
                transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles,
                    new Vector3(0, goalYRotation.Value, 0), Time.deltaTime * ROTATE_LERP_SMOOTH);
            }
            else
            {
                RotationY = (transform.parent == null) ? currentY : transform.parent.localEulerAngles.y;
                goalYRotation = null;
            }
        }
    }

    private void ObserveToTarget()
    {
        if (observeTransform != null)
        {
            var screenPoint = mainCamera.WorldToScreenPoint(observeTransform.position);
            if (!ScreenPointInCenterArea(screenPoint))
            {
                if (goalPosition.HasValue)
                    goalPosition = observeTransform.position + observeOffset;
                else
                    transform.position = observeTransform.position + observeOffset;
            }

            observeOffset = (goalPosition.HasValue)
                ? goalPosition.Value - observeTransform.position
                : transform.position - observeTransform.position;
        }
    }

    private bool ScreenPointInCenterArea(Vector3 point)
    {
        return point.x >= LEFT_X_CENTER_BORDER && point.x <= RIGHT_X_CENTER_BORDER
            && point.y >= TOP_Y_CENTER_BORDER && point.y <= BOTTOM_Y_CENTER_BORDER;
    }

    private void KeyBoardMove()
    {
        var deltaX = Input.GetAxis("Horizontal") * MOVE_SPEED;
        var deltaZ = Input.GetAxis("Vertical") * MOVE_SPEED;
        var movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, MOVE_SPEED) * Time.deltaTime;
        transform.Translate(new Vector3(movement.x, 0, movement.z));
    }

    private float GetMinRotationOffsetTarget(float currentRotation, float targetRotation)
    {
        var initialOffset = Math.Abs(currentRotation - targetRotation);
        var lessOffset = Math.Abs(currentRotation - (targetRotation - 360f));
        var moreOffset = Math.Abs(currentRotation - (targetRotation + 360f));

        var minOffset = Mathf.Min(initialOffset, lessOffset, moreOffset);
        return minOffset == lessOffset ? (targetRotation - 360f) 
            : minOffset == moreOffset ? (targetRotation + 360) : targetRotation;
    }

    private void MouseRotate()
    {
        if (Input.GetMouseButtonDown(1))
            rightMouseButtonPressed = true;
        if (Input.GetMouseButtonUp(1))
            rightMouseButtonPressed = false;
        
        if (rightMouseButtonPressed)
        {
            RotationX -= isometricCameraSensivity * rotateSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
            RotationX = Mathf.Clamp(RotationX, -80, 80);
            mainCamera.transform.localEulerAngles = new Vector3(RotationX, 0, 0);

            RotationY += isometricCameraSensivity * rotateSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
            
            transform.localEulerAngles = new Vector3(0, RotationY, 0);
        }
    }

    private void KeyBoardRotate()
    {
        var deltaX = Input.GetAxis("Horizontal") * -rotateSpeed;
        var deltaZ = Input.GetAxis("Vertical") * rotateSpeed;
        
        RotationX -= rotateSpeed * deltaZ;
        RotationX = Mathf.Clamp(RotationX, -90, 90);
        RotationY -= rotateSpeed * deltaX;
        mainCamera.transform.localEulerAngles = new Vector3(RotationX,0 , 0);
        mainCamera.transform.parent.localEulerAngles = new Vector3(0, RotationY, 0);
    }

    private void MouseWheelLift()
    {
        transform.Translate(0, LIFT_SPEED * Input.mouseScrollDelta.y * Time.deltaTime,0);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minCameraHeight, maxCameraHeight),
            transform.position.z);
    }

    private void AimMouseRotate()
    {
        RotationX -= fPCameraSensivity * rotateSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        RotationX = Mathf.Clamp(RotationX, -30, 30);
        mainCamera.transform.localEulerAngles = new Vector3(RotationX, 0, 0);

        RotationY += fPCameraSensivity * rotateSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;;
        transform.parent.localEulerAngles = new Vector3(0, RotationY, 0);
    }

    public void SaveOldIsometricCameraPosition()
    {
        if (Mode == FightCameraMode.Isometric)
        {
            oldRotationX = Instance.RotationX;
            oldRotationY = Instance.RotationY;

            var cameraObj = Camera.main.gameObject;
            oldIsometricLocalPosition = cameraObj.transform.localPosition;
            oldIsometricLocalRotate = cameraObj.transform.localEulerAngles;

            oldIsometricPosition = transform.position;
            //oldIsometricRotation = transform.eulerAngles;
        }
    }

    public void ChangeToIsometricMode(bool immediately)
    {
        if (Mode != FightCameraMode.Isometric)
        {
            StaticLineDrawer.Instance.DeleteLine();
            StartCoroutine(Instance.ChangeAfterPeriod(immediately));
        }
    }

    private IEnumerator ChangeAfterPeriod(bool immediately)
    {
        if (!immediately)
            yield return new WaitForSeconds(UIController.Instance.DamageInfoLiveSpan
                                        * UIController.Instance.DamageInfoLiveSpanPeriodCount + 0.1f);
        //var cameraObj = transform;//Camera.main.gameObject;
        //cameraObj.transform.parent = Instance.transform;
        //cameraObj.transform.localPosition = oldIsometricLocalPosition;
        //cameraObj.transform.localEulerAngles = oldIsometricLocalRotate;
        transform.parent = null;
        //transform.position = oldIsometricPosition;
        //transform.eulerAngles = oldIsometricRotation;
        //Instance.RotationX = oldRotationX;
        //Instance.RotationY = oldRotationY;
        //mainCamera.transform.localEulerAngles = new Vector3(oldRotationX, 0, 0);
        //transform.localEulerAngles = new Vector3(0, oldRotationY, 0);

        Mode = FightCameraMode.Isometric;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        
        MoveCameraTo(oldIsometricPosition);
        RotateCameraTo(new Vector3(oldRotationX, oldRotationY, 0));
    }

    public void MoveCameraToTarget(Transform target)
    {
        var height = transform.position.y; //TODO Сделать определение высоты через рейкаст(Добавить слой полу)
            //(Physics.Raycast(transform.position, Vector3.down, out var hit))
            //? Vector3.Distance(transform.position, hit.point)
            //: transform.position.y;

        var xAngle = 90 - Mathf.Abs(RotationX);
        var length = height * Mathf.Tan(xAngle * Mathf.PI / 180);
        var offsetLength = Mathf.Sqrt(length * length + height * height);
        MoveCameraTo(target.position - mainCamera.transform.forward.normalized * offsetLength);
    }

    public void MoveCameraTo(Vector3 pos)
    {
        // Изменяет position
        goalPosition = pos;
    }

    public void RotateCameraTo(Vector3 rotation)
    {
        // Изменяет localEulerAngles
        goalXRotation = rotation.x;
        goalYRotation = rotation.y;
    }

    public bool CameraReachedGoal()
    {
        return !goalPosition.HasValue && !goalXRotation.HasValue && !goalYRotation.HasValue;
    }

    public void CameraObserveTo(Transform target)
    {
        observeTransform = target;
        if (target != null)
        {
            MoveCameraToTarget(target);
            if(goalPosition.HasValue)
                observeOffset = goalPosition.Value - target.position;
            else
                observeOffset = transform.position - target.position;
        }
        //transform.localEulerAngles -= target.eulerAngles;
        //RotationY = transform.localEulerAngles.y;
        //RotateCameraTo(mainCamera.transform.localEulerAngles 
        //+ Quaternion.FromToRotation(mainCamera.transform.forward,
        //    target.position - transform.position).eulerAngles);
    }
}
