using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarScript : MonoBehaviour
{
    public int rayCount = 100;
    private Mesh mesh;
    public int rayLayerCount = 9;
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        

        float horizontalAngle = 0f;
        float horizontalAngleIncrease= 360f / rayCount;
        float verticalAngle = 45f;
        float verticalAngleIncrease = 90f / rayLayerCount;
        
        for(int i = 0; i < rayLayerCount; i++){
            for(int j = 0; j < rayCount; j++) {
                //Debug.DrawRay(transform.position, GetVectorFromAngle(verticalAngle, horizontalAngle) * 10f, Color.green);
                horizontalAngle += horizontalAngleIncrease;
            }
            verticalAngle -= verticalAngleIncrease;
        }
    }

    private Vector3 GetVectorFromAngle(float verticalAngle, float horizontalAngle)
    {
        float angleRadVertical = verticalAngle * (Mathf.PI / 180f);
        float angleRadHorizontal = horizontalAngle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(horizontalAngle),Mathf.Sin(verticalAngle), Mathf.Sin(horizontalAngle));
    }
}
