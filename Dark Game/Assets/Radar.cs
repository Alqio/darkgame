using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{

    [SerializeField] public LayerMask layerMask;
    public float radarSpeed, RADAR_MAX, TIME_AT_FULL;
    public bool fullCircle;
    public GameObject lineObject;

    private float innerDistance, radarTimer, rayLength;
    private int rayCount = 100;
    private LineRenderer lr;
    private List<GameObject> lines = new List<GameObject>();
    private bool newLine = true;
    private Vector3[] outerCircle;
    private List<GameObject> foundObjects = new List<GameObject>();
    private List<string> foundIds = new List<string>();
    void Start()
    {
        innerDistance = radarTimer = rayLength = 0f;
        lr = GetComponent<LineRenderer>();
        lr.positionCount = rayCount + 1;
        lr.loop = true;
        outerCircle = new Vector3[rayCount+1];
    }

    void Update()
    {

        radarTimer += Time.deltaTime;
        if(radarTimer < RADAR_MAX){
            rayLength += radarSpeed *  Time.deltaTime;
            if(fullCircle){
                lr.SetPositions(GetOutlineFull());
            }else{
                UpdateOutlineWalls();
                lr.SetPositions(outerCircle);
            }
        } else if(radarTimer > TIME_AT_FULL - RADAR_MAX){
            //Not needed with current radar implementation
            /*
            if(false){
                flag = false;
                GameObject rLine = Instantiate(lineObject);
                rLine.transform.parent = transform;
                Vector3[] positions = new Vector3[lr.positionCount];
                lr.GetPositions(positions);
                rLine.GetComponent<LineRenderer>().positionCount = positions.Length;
                rLine.GetComponent<LineRenderer>().SetPositions(positions);
                lines.Add(rLine);
                Destroy(lr);
            }
            */
            
            if(innerDistance < rayLength){
                innerDistance += radarSpeed * Time.deltaTime;
                FilterLinePoints();
            } else {
                Destroy(gameObject);
            }
            
        }
    }

    private void FilterLinePoints()
    {      
        List<GameObject> tmp_gos = new List<GameObject>();
        foreach(GameObject go in lines)
        {
            LineRenderer line = go.GetComponent<LineRenderer>();
            Vector3[] positions = new Vector3[line.positionCount];
            line.GetPositions(positions);
            List<Vector3> line_tmp = new List<Vector3>();
            LineRenderer lr_tmp;
            foreach( Vector3 pos in positions)
            {
                //TODO: store distance, don't calculate every time
                if(Vector2.Distance(pos, transform.position) > innerDistance) {
                    //Is fine
                    line_tmp.Add(pos);
                } else {
                    //Too close, forget
                    GameObject newObject = Instantiate(lineObject);
                    lr_tmp = newObject.GetComponent<LineRenderer>();
                    lr_tmp.positionCount = line_tmp.Count;
                    lr_tmp.SetPositions(line_tmp.ToArray());
                    newObject.transform.parent = transform;
                    tmp_gos.Add(newObject);
                    line_tmp.Clear();
                }
            }
            if(line_tmp.Count > 0){
                GameObject newObject = Instantiate(lineObject);
                lr_tmp = newObject.GetComponent<LineRenderer>();
                lr_tmp.positionCount = line_tmp.Count;
                lr_tmp.SetPositions(line_tmp.ToArray());
                newObject.transform.parent = transform;
                tmp_gos.Add(newObject);
                line_tmp.Clear();
        
            }
        }
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
        lines = tmp_gos;
        

        List<GameObject> tmp_found = new List<GameObject>(); 
        foreach(GameObject mock in foundObjects)
        {
            if(Vector2.Distance(mock.transform.position, transform.position) < innerDistance){

                Destroy(mock);
            } else {
                tmp_found.Add(mock);
            }
        }
        foundObjects = tmp_found;
    }

    private Vector3[] GetOutlineFull()
    {
        float angle = 0f;
        float angleIncrease = 360f / rayCount;

        Vector3[] points = new Vector3[rayCount];
        for(int i = 0; i < rayCount; i++){
            Vector3 point;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetVectorFromAngle(angle), rayLength, layerMask);
            if(raycastHit2D.collider == null){
                // No hit
                point = GetVectorFromAngle(angle) * rayLength;
            } else {
                //Hit object
                point = raycastHit2D.point - new Vector2(transform.position.x, transform.position.y);
            }
            points[i] = point;
            
            angle -= angleIncrease;

        }
        
        return points;
    }
    private Vector3[] UpdateOutlineWalls()
    {
        
        foreach (GameObject go in lines)
        {
            Destroy(go);
        }
        lines.Clear();
        newLine = true;

        float angle = 0f;
        float angleIncrease = 360f / rayCount;

        Vector3[] points = new Vector3[rayCount];
        Vector3 prevPoint = Vector3.zero;
        for(int i = 0; i < rayCount + 1; i++){
            outerCircle[i] = GetVectorFromAngle(angle) * rayLength + transform.position;
            Vector3 point = Vector3.zero;


            RaycastHit2D[] raycastHit2Darr = Physics2D.RaycastAll(transform.position, GetVectorFromAngle(angle), rayLength, layerMask);
            bool hasWallHit = false;
            foreach(RaycastHit2D hit in raycastHit2Darr){
                point = hit.point;
                if(hit.transform.tag == "Wall"){
                    //Hit Wall
                    
                    LineRenderer lr_tmp;
                    if(Vector2.Distance(transform.position, point) > innerDistance){

                        if(newLine || (i > 0 && Vector2.Distance(prevPoint, point) > 1f)){
                            GameObject go = Instantiate(lineObject);
                            lr_tmp = go.GetComponent<LineRenderer>();
                            lr_tmp.positionCount = 0;
                            go.transform.parent = transform;
                            lines.Add(go);

                            newLine = false;
                        }else{
                            lr_tmp = lines[lines.Count - 1].GetComponent<LineRenderer>();
                        }
                        lr_tmp.positionCount = lr_tmp.positionCount + 1;
                        lr_tmp.SetPosition(lr_tmp.positionCount - 1, point);
                        prevPoint = point;
                    }
                    //points[i] = point;
                    hasWallHit = true;
                    break;
                } else if(hit.transform.tag == "ShowInRadar") {
                    //save object
                    string hitId = hit.transform.gameObject.GetComponent<UniqueId>().ID;
                    if(!foundIds.Contains(hitId)){
                        foundIds.Add(hitId);
                        GameObject mock = new GameObject("mock");
                        mock.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, 0);
                        mock.AddComponent<SpriteRenderer>().sprite = hit.transform.gameObject.GetComponent<SpriteRenderer>().sprite;
                        mock.transform.parent = transform;
                        mock.layer = LayerMask.NameToLayer("Default");
                        foundObjects.Add(mock);
                    }

                }
            }
            if(!hasWallHit){
                // No hit
                point = GetVectorFromAngle(angle) * rayLength;
                newLine = true;
            }

            angle -= angleIncrease;

        }
        return points;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

}
