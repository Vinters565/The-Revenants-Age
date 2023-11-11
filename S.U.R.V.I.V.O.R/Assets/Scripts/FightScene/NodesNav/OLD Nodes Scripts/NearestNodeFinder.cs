using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearestNodeFinder : MonoBehaviour
{
    public GameObject TriggerPrefab;
    public GameObject NearestNode;
    private GameObject trigger;

    public void FindNearNode(Vector3 position)
    {
        trigger = Instantiate(TriggerPrefab, position, Quaternion.identity);
        trigger.GetComponent<SphereCollider>().radius = 5f;
    }

    void Start()
    {
    }

    void Update()
    {
        if(trigger != null && trigger.GetComponent<Trigger>().NearestNode != null)
        {
            NearestNode = trigger.GetComponent<Trigger>().NearestNode;
            Destroy(trigger);
        }
    }

}
