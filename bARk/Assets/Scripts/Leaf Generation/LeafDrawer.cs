using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeafDrawer : MonoBehaviour {

    public List<Vector3> points = new List<Vector3>();
    public float newPointsMinDist = 0.2f;
    public float endSnapDistance = 0.5f;
    private Vector3 endPoint;
    private LineRenderer lineRenderer;

    public GameObject meshObject;

    private bool leafCreated = false;

    public float zDist = 10;

    // Use this for initialization
    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(0.1f, 0.1f);


        Vector3 screenMidBottom = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.48f, Screen.height * 0.2f, zDist));
        Vector3 screenMidBottom2 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.52f,Screen.height * 0.2f, zDist));
        Vector3 screenMidMid = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.48f, Screen.height * 0.4f, zDist));
        endPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.52f, Screen.height * 0.4f, zDist));
        points.Add(endPoint);
        points.Add(screenMidBottom2);
        points.Add(screenMidBottom);
        points.Add(screenMidMid);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && !leafCreated) { 
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDist);
            Vector3 drawPos = Camera.main.ScreenToWorldPoint(mousePos);
            print(isOverlapping(drawPos));
            if ((drawPos-endPoint).magnitude <= endSnapDistance && points.Count > 5) {
                points.Add(new Vector3(endPoint.x, endPoint.y, endPoint.z));
                finalizeLeaf();
            }   else if (!pointToClose(drawPos) && !isOverlapping(drawPos)) {
                points.Add(drawPos);
            }
        }
        lineRenderer.SetVertexCount(points.Count);
        lineRenderer.SetPositions(points.ToArray());

	}

    private void finalizeLeaf() {
        LeafGenerator lg = new LeafGenerator();
        Vector3[] reversedPoints = new Vector3[points.Count];
        Mesh leaf = lg.generateLeafMesh(points.ToArray());
        GameObject newLeaf = Instantiate(meshObject);
        Mesh invertedLeaf = reverseNormals(leaf);
        newLeaf.GetComponent<MeshFilter>().mesh = leaf;
        newLeaf.GetComponent<MeshCollider>().sharedMesh = leaf;
        leafCreated = true;
        newLeaf.transform.position = Vector3.zero;
        Destroy(gameObject);
       // newLeaf.transform.localScale = new Vector3(-1, 1, 1);
    }

    private bool isOverlapping(Vector3 pos) {
        Vector3 from = points[points.Count - 1];
        for (int i = 0; i < points.Count-2; ++i) {
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            if (Mathf.Max(from.x,pos.x) >= Mathf.Min(p1.x,p2.x) && Mathf.Max(p1.x,p2.x) >= Mathf.Min(from.x,pos.x)
                && Mathf.Max(from.y,pos.y) >= Mathf.Min(p1.y,p2.y) && Mathf.Max(p1.y,p2.y) >= Mathf.Min(from.y,pos.y)) {
                return true;
            }
        }
        return false;
    }

    private Mesh reverseNormals(Mesh mesh) {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;

        for (int m = 0; m < mesh.subMeshCount; m++) {
            int[] triangles = mesh.GetTriangles(m);
            for (int i = 0; i < triangles.Length; i += 3) {
                int temp = triangles[i + 0];
                triangles[i + 0] = triangles[i + 1];
                triangles[i + 1] = temp;
            }
            mesh.SetTriangles(triangles, m);
        }
        return mesh;
    }

    private bool between(float value, float from, float to) {
        return (value <= from && value >= to) || (value >= from && value <= to); 
    }

    private bool pointToClose(Vector3 point) {
        bool tooClose = false;
        foreach (Vector3 p in points) {
            if ((point - p).magnitude < newPointsMinDist)
                tooClose = true;
        }
        return tooClose;
    }

}
