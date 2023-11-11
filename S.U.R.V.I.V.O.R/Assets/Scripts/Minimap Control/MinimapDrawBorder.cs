using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapDrawBorder : MonoBehaviour
{
    private new Camera camera;
    private Camera minimapCamera;
    private LineRenderer lr;
    [SerializeField] private GameObject minimapTexture;

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        minimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<Camera>();
        lr = GetComponent<LineRenderer>();
        lr.sharedMaterial.SetColor("_Color", Color.black);
    }

    private void Update()
    {
        DrawLine();
    }

    private Vector3 GetCameraFrustumPoint(Vector3 position)
    {
        var positionRay = camera.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(positionRay, out hit, Mathf.Infinity))
            return hit.point;
        return new Vector3();
    }


    public void DrawLine()
    {
        Vector3 minPoint = GetCameraFrustumPoint(new Vector3(0f,0f));
        Vector3 maxPoint = GetCameraFrustumPoint(new Vector3(Screen.width,Screen.height));

        var texture = minimapTexture.GetComponent<RawImage>().texture;
        var rect = minimapTexture.GetComponent<RectTransform>().rect;
        float minX = minPoint.x;
        float minY = minPoint.z;

        float maxX = maxPoint.x;
        float maxY = maxPoint.z;
        
        var points = new []
        {
            new Vector3(minX, 5, minY), new Vector3(minX, 5, maxY), new Vector3(maxX, 5, maxY),
            new Vector3(maxX, 5, minY)
        };
        lr.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i]);
        }

        // GL.Begin(GL.LINES);
        // GL.PushMatrix();
        // {
        //     GL.LoadOrtho();
        //
        //     GL.Begin(GL.QUADS);
        //     GL.Color(Color.red);
        //     {
        //         cameraBoxMaterial.SetPass(0);
        //         GL.Vertex(new Vector3(minX, minY + lineWidth, 0));
        //         GL.Vertex(new Vector3(minX, minY - lineWidth, 0));
        //         GL.Vertex(new Vector3(maxX, minY - lineWidth, 0));
        //         GL.Vertex(new Vector3(maxX, minY + lineWidth, 0));
        //
        //         GL.Vertex(new Vector3(minX + lineWidth, minY, 0));
        //         GL.Vertex(new Vector3(minX - lineWidth, minY, 0));
        //         GL.Vertex(new Vector3(minX - lineWidth, maxY, 0));
        //         GL.Vertex(new Vector3(minX + lineWidth, maxY, 0));
        //
        //         GL.Vertex(new Vector3(minX, maxY + lineWidth, 0));
        //         GL.Vertex(new Vector3(minX, maxY - lineWidth, 0));
        //         GL.Vertex(new Vector3(maxX, maxY - lineWidth, 0));
        //         GL.Vertex(new Vector3(maxX, maxY + lineWidth, 0));
        //
        //         GL.Vertex(new Vector3(maxX + lineWidth, minY, 0));
        //         GL.Vertex(new Vector3(maxX - lineWidth, minY, 0));
        //         GL.Vertex(new Vector3(maxX - lineWidth, maxY, 0));
        //         GL.Vertex(new Vector3(maxX + lineWidth, maxY, 0));
        //     }
        //     GL.End();
        // }
        // GL.PopMatrix();
    }
}