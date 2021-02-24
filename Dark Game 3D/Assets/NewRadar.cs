using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRadar : MonoBehaviour
{
    private float timer, radius;
    public float  speed, maxRadius = 20f, timeAtMax = 5f;

   
    //float radius;
    void Start()
    {
        radius = 0f;
        Shader.SetGlobalVector("_Position", transform.position);
    }

    void Update()
    {
        //radius = transform.localScale.x/2;
        //GetComponent<Renderer>().material.SetVector("_Position", transform.position);
        //GetComponent<Renderer>().material.SetFloat("_Radius", radius);
        if(radius < maxRadius) {
            radius += speed * Time.deltaTime;
            transform.localScale = Vector3.one * radius;
            Shader.SetGlobalFloat("_Radius", radius);
        } else {
            timer += Time.deltaTime;
            if(timer > timeAtMax){
                Shader.SetGlobalFloat("_Radius", 0);
                Destroy(gameObject);
            }
        }
        /*
        */
    }
}
