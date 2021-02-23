using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarCollision : MonoBehaviour
{
    List<ContactPoint> contacts = new List<ContactPoint>();
    public LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        //lr = GetComponent<LineRenderer>();
        Physics.reuseCollisionCallbacks = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision col){
        col.GetContacts(contacts);
        //Debug.Log(contacts.Length);
        Debug.Log(col.contactCount);
/*
        foreach (var item in contacts)
        {
            Debug.Log(item.point);
        }
  */      
        lr.positionCount = contacts.Count;
        for(int i = 0; i < contacts.Count; i++)
        {   
            lr.SetPosition(i, contacts[i].point);
        }
    }
}
