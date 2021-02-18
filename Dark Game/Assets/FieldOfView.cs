using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    THIS MIGHT BE BROKEN, NOT TESTED!!!
*/

public class FieldOfView : MonoBehaviour
{

    [SerializeField] public LayerMask layerMask;
    private Mesh mesh;
    public float radarSpeed, MAX_TIME;
    Vector3 origin;
    private float inner, outer, radarTimer;

    private float rayLength = 0f;
    private int rayCount = 50;

    public LineRenderer line;
    private LineRenderer lr;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        

    }

    void Update()
    {

        if(Input.GetKey(KeyCode.Q)){
            radarTimer = MAX_TIME;
        }

        if(radarTimer > 0f){
            outer += radarSpeed *  Time.deltaTime;
            radarTimer -= Time.deltaTime;
        } else if(inner < outer) {
            inner += radarSpeed * Time.deltaTime;
        }else{
            inner = 0f;
            outer = 0f;
        }
        
    }

    void LateUpdate()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        float angle = 0f;
        float angleIncrease = 360f / rayCount;

        Vector3[] verticies = new Vector3[rayCount * 2];
        Vector2[] uv = new Vector2[verticies.Length];
        int[] triangles = new int[rayCount * 3 * 2];

        for (int i = 0; i < verticies.Length; i+=2)
        {
            Vector3 pointOnOuter, pointOnInner;

            RaycastHit2D outerRaycastHit2D = Physics2D.Raycast(transform.position, GetVectorFromAngle(angle), outer, layerMask);
            if (outerRaycastHit2D.collider == null)
            {
                //No hit
                pointOnOuter = GetVectorFromAngle(angle) * outer;
            }
            else
            {
                //Hit object
                pointOnOuter = outerRaycastHit2D.point - new Vector2(transform.position.x, transform.position.y);
            }

            RaycastHit2D innerRaycastHit2D = Physics2D.Raycast(transform.position, GetVectorFromAngle(angle), inner, layerMask);
            if (innerRaycastHit2D.collider == null)
            {
                //No hit
                pointOnInner = GetVectorFromAngle(angle) * inner;
            }
            else
            {
                //Hit object
                pointOnInner = innerRaycastHit2D.point - new Vector2(transform.position.x, transform.position.y);
            }

            verticies[i] = pointOnInner;
            verticies[i + 1] = pointOnOuter;
            
            triangles[i * 3] = i; 
            triangles[i * 3 + 1] = i + 1; 
            triangles[i * 3 + 3] = i + 1; 
            if(i < verticies.Length - 2){
                triangles[i * 3 + 2] = i + 2; 
                triangles[i * 3 + 4] = i + 3; 
                triangles[i * 3 + 5] = i + 2; 
            } else {
                triangles[i * 3 + 2] = 0; 
                triangles[i * 3 + 4] = 1; 
                triangles[i * 3 + 5] = 0; 

            }

            angle -= angleIncrease;
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);

    }

    public void SetOrigin(Vector3 newOrigin)
    {
        origin = newOrigin;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}