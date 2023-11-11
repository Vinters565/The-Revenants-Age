using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

using TheRevenantsAge;


public class GunAimer : Aimer
{
    private const float FP_CAMERA_OFFSET = 1.5f;
    private const float IGNORE_RAYCAST_DISTANCE = 1f;
    private const float MINIMAL_HIT_DISTANCE = 2f;
    private const float RAYCAST_MAX_DISTANCE = 200f;
    

    [SerializeField] private GameObject aimPointPrefab;
    
    private Transform gunPoint;
    private FightCameraMode aimMode = FightCameraMode.FPShooting;
    private Camera mainCamera;
    
    public GameObject AimPoint { get; private set; }

    private void Start()
    {
        gunPoint = transform.Find("GunPoint");
        AimPoint = Instantiate(aimPointPrefab, Vector3.zero, Quaternion.identity);
        AimPoint.SetActive(false);
        mainCamera = Camera.main;
    }

    public override void ChangeCameraToAimMode()
    {
        CameraScript.Instance.Mode = aimMode;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void ChangeCameraPosition(Transform characterObj)
    {
        CameraScript.Instance.RotationX = characterObj.localEulerAngles.x;
        CameraScript.Instance.RotationY = characterObj.localEulerAngles.y;
        var cameraPanelTransform = CameraScript.Instance.transform;//Camera.main.gameObject.transform;
        var cameraTransform = Camera.main.gameObject.transform;
        
        cameraPanelTransform.parent = characterObj;
        //cameraPanelTransform.localPosition = Vector3.zero;
        //cameraPanelTransform.localEulerAngles = Vector3.zero;
        //cameraTransform.localEulerAngles = Vector3.zero;

        var FPCameraOffset = characterObj.forward * -FP_CAMERA_OFFSET +
                             characterObj.right * FP_CAMERA_OFFSET / 2 +
                             characterObj.up * FP_CAMERA_OFFSET;
        //cameraPanelTransform.Translate(Vector3.forward * -FP_CAMERA_OFFSET);
        //cameraPanelTransform.Translate(Vector3.right * FP_CAMERA_OFFSET / 2);
        //cameraPanelTransform.Translate(Vector3.up * FP_CAMERA_OFFSET);
        CameraScript.Instance.MoveCameraTo(characterObj.position + FPCameraOffset);
        CameraScript.Instance.RotateCameraTo(Vector3.zero);
    }

    public override void DrawAimLine(Vector3 targetPoint)
    {
        AimPoint.SetActive(true);
        AimPoint.transform.position = targetPoint;
        AimPoint.transform.localScale = Vector3.one * (0.01f + Vector3.Distance(targetPoint, gunPoint.position) / 75);
    }

    public override Vector3 Aim()
    {
        var point = new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, 0);
        var cameraRay = mainCamera.ScreenPointToRay(point);
        var wasHitted = Physics.Raycast(cameraRay, out var hit);
        var origin = gunPoint.position;

        while (wasHitted && hit.distance <= IGNORE_RAYCAST_DISTANCE * FP_CAMERA_OFFSET)
        {
            var obj = hit.transform.gameObject;
            var oldLayer = obj.layer;
            obj.layer = LayerMask.NameToLayer("Ignore Raycast");
            StartCoroutine(SetDefaultLayer(oldLayer, obj));

            wasHitted = Physics.Raycast(cameraRay, out hit);
        }

        if (wasHitted && hit.distance <= MINIMAL_HIT_DISTANCE)
        {
            hit = new RaycastHit();
            hit.point = origin + mainCamera.transform.forward * MINIMAL_HIT_DISTANCE;
        }

        if (!wasHitted)
        {
            hit = new RaycastHit();
            hit.point = origin + mainCamera.transform.forward * RAYCAST_MAX_DISTANCE;
        }
        
        var direction = hit.point - origin;
        Physics.Raycast(origin, direction, out var aimHit);
        if (aimHit.collider == null)
        {
            aimHit = new RaycastHit();
            aimHit.point = origin + direction * RAYCAST_MAX_DISTANCE;
        }

        return aimHit.point;
    }

    public IEnumerator SetDefaultLayer(int layer, GameObject obj)
    {
        yield return new WaitForSeconds(5f);
        obj.layer = layer;
    }

    public override void StopDrawAiming()
    {
        AimPoint.SetActive(false);
    }
}
