using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRadar : MonoBehaviour
{
    private float timer, radius;
    public float  speed, maxRadius = 20f, timeAtMax = 5f;

    private bool isShrinking = false;

    public bool DEBUG;
   
    //float radius;
    void Start()
    {
        radius = 0f;
        Shader.SetGlobalVector("_Position", transform.position);
    }

    void Update()
    {
        if(DEBUG){

            radius = transform.localScale.x/2;
            Shader.SetGlobalVector("_Position", transform.position);
            Shader.SetGlobalFloat("_Radius", radius);
        } else {
            if(radius < maxRadius && !isShrinking) {
                radius += speed * Time.deltaTime;
                transform.localScale = Vector3.one * radius* 2;
                Shader.SetGlobalFloat("_Radius", radius);
            } else {
                isShrinking = true;
                timer += Time.deltaTime;
                if(timer > timeAtMax){
                    if(radius > 0) {
                        radius -= speed * Time.deltaTime;
                        radius = Mathf.Max(0f, radius);
                        transform.localScale = Vector3.one * radius * 2;
                        //Debug.Log(radius);
                        Shader.SetGlobalFloat("_Radius", radius);
                    } else {
                        Shader.SetGlobalFloat("_Radius", 0);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
