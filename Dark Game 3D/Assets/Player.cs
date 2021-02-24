using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject radar;

    // Start is called before the first frame update
    void Start()
    {
        Shader.SetGlobalFloat("_Radius", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            Instantiate(radar, transform.position, transform.rotation);
        }
    }
}
