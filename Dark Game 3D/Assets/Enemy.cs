using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float rotationSpeed, speed, startWaitTime;
    private float waitTime;
    public Transform[] patrolPositions = new Transform[2];
    private int curr; 
    private Quaternion targetRotation;
    void Start() {
        curr = 0;
        targetRotation = Quaternion.LookRotation(patrolPositions[curr].position - transform.position);
        waitTime = 0f;
    }

    void Update() {
        if(transform.rotation == targetRotation){
            transform.position = Vector3.MoveTowards(transform.position, patrolPositions[curr].position, speed * Time.deltaTime);
        } else {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        if(Vector3.Distance(transform.position, patrolPositions[curr].position) < 0.01f) {
            if(curr == 0) curr = 1;
            else if(curr == 1) curr = 0;
            //waitTime = startWaitTime;
            targetRotation = Quaternion.LookRotation(patrolPositions[curr].position - transform.position);
        }
    }
}
