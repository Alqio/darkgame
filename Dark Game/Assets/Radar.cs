using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{

    [SerializeField] public LayerMask layerMask;
    private Mesh mesh;
    public float inner, outer;
    Vector3 origin;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void LateUpdate()
    {
        //origin = ;
        //Debug.Log(origin);
        //Debug.Log(transform.TransformPoint(origin));
        int rayCount = 7;
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
            if (outerRaycastHit2D.collider == null)
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
            /*
            if (i == 0) {
                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
            } else {
                triangles[triangleIndex] = ;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;

            }
*/
            angle -= angleIncrease;
        }

        //012 123 234 345 456 567 678 789
        for(int i = 0; i < rayCount; i++){
            triangles[i * 4] = i*2; 
            triangles[i * 4 + 1] = i*2 + 1; 
            if(i < rayCount - 1){
                triangles[i * 4 + 2] = i*2 + 3; 
            } else {
                triangles[i * 4 + 2] = 1; 
            }
            triangles[i * 4 + 3] = i*2;
            
        }
/*
        foreach (var item in verticies)
        {
            Debug.Log("v: " + item);
        }
*/
        foreach (var item in triangles)
        {
            Debug.Log("p: " + item);
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
