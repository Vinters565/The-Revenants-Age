using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraColliderScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var mesh = other.GetComponent<MeshRenderer>();
        if(mesh)
            mesh.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        var mesh = other.GetComponent<MeshRenderer>();
        if(mesh)
            mesh.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        var mesh = other.GetComponent<MeshRenderer>();
        if(mesh)
            mesh.enabled = true;
    }
}

