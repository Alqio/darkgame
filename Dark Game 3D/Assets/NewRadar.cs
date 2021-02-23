using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRadar : MonoBehaviour
{
    private float size, timer;
    public float speed, maxSize = 10f, timeAtMax = 5f;


    void Start()
    {
        size = 0f;
    }

    void Update()
    {
        if(size < maxSize) {
            size += speed * Time.deltaTime;
            transform.localScale = Vector3.one * size;
        } else {
            timer += Time.deltaTime;
            if(timer > timeAtMax){
                Destroy(gameObject);
            }
        }
    }
}
