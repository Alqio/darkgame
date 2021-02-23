using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CustomPoint
{
    public bool used;
    public Vector3 coords;
}

struct ConePoint
{
   public bool isDone;
   public Vector3 point;
}

public class RadarScript : MonoBehaviour
{
    public int rayCount = 100;
    [Range(0f,2f)] public float treshold = 0.1f;
    [Range(1,10)] public int inverseResolution = 3;
    private Mesh mesh;
    public int rayLayerCount = 9;
    public AnimationCurve rayLength;
    public  LayerMask layerMask;

    private float timer = 0;

    LineRenderer lr;
    private List<Vector3> points = new List<Vector3>();

    public bool useSorting = false;

    private List<Vector3> rayDirections = new List<Vector3>();
    [Range(1f,20f)] public float coneSize = 1f;
    [Range(1f,20f)] public float maxConeSize = 1f;
    [Range(1f,10f)] public float coneSizeIncrease = 1f;
    private List<Vector3> crossHelpers = new List<Vector3>();



    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        lr = GetComponent<LineRenderer>();

        for(int x = -1; x <= 1; x++){
            for(int y = -1; y <= 1; y++){
                for(int z = -1; z <= 1; z++){
                    rayDirections.Add(new Vector3(x,y,z));
                }
            }
        }
    }

    void Update()
    {

        StartCoroutine("CastRaysGrowingCone");
        timer += Time.deltaTime;
  /*
        sphere.transform.localScale = new Vector3(currentRayLength, currentRayLength, currentRayLength );
        Debug.Log(currentRayLength);
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, currentRayLength, Vector3.forward, 10000f,layerMask, QueryTriggerInteraction.UseGlobal);
        List<Vector3> points = new List<Vector3>();
        Debug.Log(raycastHits.Length);
        foreach (RaycastHit hit in raycastHits)
        {
            Debug.Log(hit);
            if(Mathf.Abs(Vector3.Distance(transform.position, hit.point) - currentRayLength) < 0.1f)
            {
            }
                points.Add(hit.point);
        }
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        timer += Time.deltaTime;
*/
/*
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
    */
    }

    IEnumerator CastRaysAll() {
        
        points.Clear();
        float currentRayLength = rayLength.Evaluate(timer);
        
        Vector3 direction = Vector3.right;
        int steps = Mathf.FloorToInt(360f / inverseResolution);
        Quaternion xRotation = Quaternion.Euler(Vector3.right * inverseResolution);
        Quaternion yRotation = Quaternion.Euler(Vector3.up * inverseResolution);
        Quaternion zRotation = Quaternion.Euler(Vector3.forward * inverseResolution);

        List<CustomPoint> customPoints = new List<CustomPoint>();
        for(int x=0; x < steps/2; x++) {
            direction = zRotation * direction;
            for(int y=0; y < steps; y++) {
                direction = xRotation * direction;
                Debug.DrawLine(transform.position, transform.position + direction * currentRayLength, Color.red); // for science
                if(Physics.Raycast(transform.position, transform.position + direction * currentRayLength, out RaycastHit hitInfo)){
                    if(Mathf.Abs(hitInfo.distance - currentRayLength) < treshold)
                    {
                        CustomPoint p = new CustomPoint();
                        p.used = false;
                        p.coords = hitInfo.point;
                        customPoints.Add(p);
                        points.Add(hitInfo.point);
                    }
                }
            }
        }
        if(customPoints.Count > 0 && useSorting){
            Vector3[] sortedArr = SortPoints(customPoints.ToArray());
            lr.positionCount = sortedArr.Length;
            lr.SetPositions(sortedArr);
        } else {
            lr.positionCount = points.Count;
            lr.SetPositions(points.ToArray());
        }
        yield return null;
    }

    IEnumerator CastRaysGrowingCone() {
        float currentRayLength = rayLength.Evaluate(timer);

        ConePoint[,] conePoints = new ConePoint[rayDirections.Count,8];
        List<Vector3> newRays = new List<Vector3>();
        for(int i = 0; i < rayDirections.Count; i++)
        {
            Vector3 direction = Vector3.forward;
            Debug.DrawLine(transform.position, transform.position + direction * currentRayLength, Color.red); // for science
            if(Physics.Raycast(transform.position, transform.position + direction * currentRayLength, out RaycastHit hitInfo))
            {            
                for(int j = 0; j < rayDirections.Count; j++)
                {
                    Vector3 helper = rayDirections[j];
                    Vector3 cross = Vector3.Cross(direction, helper);
                    //Debug.Log("cross: " + cross + " dir: " + direction + " helper: " + helper);
                    bool sameDir = Vector3.Equals(direction.normalized, helper.normalized) || Vector3.Equals(-direction.normalized, helper.normalized) || Vector3.Equals(cross.normalized, Vector3.zero);
                    if(!sameDir){
                        
                        while(!conePoints[i, j].isDone && coneSize > maxConeSize){
                            Debug.DrawLine(transform.position, transform.position +  direction * currentRayLength + Vector3.Cross(direction, helper).normalized * coneSize, Color.green); // for science
                            if(Physics.Raycast(transform.position, transform.position + direction * currentRayLength+ Vector3.Cross(direction, helper).normalized * coneSize, out RaycastHit coneHitInfo)) {
                                conePoints[i, j].point = coneHitInfo.point;
                                if(coneHitInfo.distance < treshold){
                                    conePoints[i, j].isDone = true;
                                }
                            }
                            coneSize += coneSizeIncrease * Time.deltaTime;
                        }
                    }
                }
            }
        }
        rayDirections.Clear();
        foreach(ConePoint conePointDir in conePoints)
        {
            rayDirections.Add((conePointDir.point - transform.position).normalized);
        }
        lr.positionCount = rayDirections.Count;
        lr.SetPositions(rayDirections.ToArray());
        yield return null;
    }

    Vector3[] SortPoints(CustomPoint[] start)
    {
        Vector3[] sortedArr = new Vector3[start.Length];
        Vector3 myOrigin = start[0].coords;
        for (int i = 0; i < start.Length; i++)
        {   
            int bestPointIndex = 0;
            float bestDistance = Mathf.Infinity;
            for(int j = 0; j < start.Length; j++)
            {
                
                if(!start[j].used)
                {
                    float thisDistance = Vector3.Distance(myOrigin, start[j].coords);
                    if(thisDistance > 0f && thisDistance < bestDistance)
                    {
                        bestPointIndex = j;
                        bestDistance = thisDistance;
                    }
                }
            }

            sortedArr[i] = start[bestPointIndex].coords;
            start[bestPointIndex].used = true;
            Debug.Log(i + " curr: " + myOrigin + " best: " + start[bestPointIndex].coords +  " dist: " + bestDistance );
            myOrigin = sortedArr[i];
        }
        return sortedArr;
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        //Debug.DrawLine(transform.position, )
        //Gizmos.DrawWireSphere(transform.position, rayLength.Evaluate(timer));
    }

    private Vector3 GetVectorFromAngle(float verticalAngle, float horizontalAngle)
    {
        float angleRadVertical = verticalAngle * (Mathf.PI / 180f);
        float angleRadHorizontal = horizontalAngle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(horizontalAngle),Mathf.Sin(verticalAngle), Mathf.Sin(horizontalAngle));
    }
}
