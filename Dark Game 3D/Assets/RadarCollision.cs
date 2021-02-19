using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarCollision : MonoBehaviour
{
    ContactPoint[] contacts = new ContactPoint[1000];
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
        Physics
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
        lr.positionCount = contacts.Length;
        for(int i = 0; i < contacts.Length; i++)
        {   
            lr.SetPosition(i, contacts[i].point);
        }
    }
}
