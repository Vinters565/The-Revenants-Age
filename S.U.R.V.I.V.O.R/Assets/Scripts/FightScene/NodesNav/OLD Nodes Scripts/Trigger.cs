using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject NearestNode;
    private float minDistance;
    // Start is called before the first frame update
    void Start()
    {
        minDistance = float.MaxValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) 
    {
        if (other.GetComponent<FightNode>() != null)
        {
            var dx = transform.position.x - other.transform.position.x;
            var dy = transform.position.y - other.transform.position.y;
            var dz = transform.position.z - other.transform.position.z;
            var distance = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
            if (distance < minDistance)
            {
                minDistance = distance;
                NearestNode = other.gameObject;
            }
        }
    }
}
