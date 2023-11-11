    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MinimapController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RawImage minimapTexture;

    private Camera minimapCamera;
    private LineRenderer lr;
    
    public event Action<Vector3> MoveToDestinationEvent;
    
    public bool isActive { get; set; }
    
    private static MinimapController instance;
    
    public static MinimapController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(MinimapController)) as MinimapController;
                if (instance == null)
                {
                    instance = new GameObject("MinimapController").AddComponent<MinimapController>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<Camera>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapTexture.rectTransform,
                eventData.pressPosition, eventData.pressEventCamera, out var cursor))
        {
            var texture = minimapTexture.texture;
            var rect = minimapTexture.rectTransform.rect;

            var calX = ((cursor.x - rect.x) * texture.width) / rect.width;
            var calY = ((cursor.y - rect.y) * texture.height) / rect.height;

            cursor = new Vector2(calX, calY);

            var point = CastRayToWorld(cursor);
            MoveToDestinationEvent?.Invoke(new Vector3(point.x, transform.position.y, point.z));
        }
    }

    private Vector3 CastRayToWorld(Vector2 vec)
    {
        var mapRay = minimapCamera.ScreenPointToRay(vec);

        RaycastHit miniMapHit;

        if (Physics.Raycast(mapRay, out miniMapHit, Mathf.Infinity))
        {
            var mPos = miniMapHit.point;
            return mPos;
        }

        return new Vector3();
    }
}