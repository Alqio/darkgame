using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject radar;
    public Transform tazer;
    public Transform helper;
    public LayerMask layerMask;
    private LineRenderer tazerLine;
    private float tazerTimer;
    public Text scoreText;
    private int score;
    void Start()
    {
        score = 0;
        scoreText.text = "Score: " + score; 
        Shader.SetGlobalFloat("_Radius", 0);
        tazerLine = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            Instantiate(radar, transform.position, transform.rotation);
        }
        tazerLine.SetPosition(0, tazer.position);
        if(Input.GetMouseButtonDown(0)){
            //Debug.Log("TAZERRR!!" + tazer.position + tazer.forward * 10f);
            //Debug.DrawLine(tazer.position, tazer.rotation * tazer.forward * 10f, Color.green );
            tazerLine.positionCount = 2;
            tazerTimer = 0.3f;
            if(Physics.Raycast(tazer.position, (tazer.position - helper.position), out RaycastHit hitInfo,  10f, layerMask)) {
                Debug.Log("hit " + hitInfo.collider.gameObject);
                score += 100;
                scoreText.text = "Score: " + score;
                Destroy(hitInfo.collider.transform.parent.gameObject);
            }
        }
        if(tazerTimer > 0f){
            tazerTimer -= Time.deltaTime;
            tazerLine.SetPosition(1, tazer.position + (tazer.position - helper.position) * 10f);

        }else{
            tazerLine.positionCount = 1;
        }
    }
}
