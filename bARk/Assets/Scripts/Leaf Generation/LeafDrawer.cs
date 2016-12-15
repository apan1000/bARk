using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LeafDrawer : MonoBehaviour {

    public List<Vector3> points = new List<Vector3>();
    public float newPointsMinDist = 0.2f;
    public float endSnapDistance = 0.5f;
    public float stemWidth = 0.02f;
    public float lineHeight = 0.2f;
    private Vector3 endPoint;
    private LineRenderer lineRenderer;
    public float drawingStartDelay = 1.0f;
    public float drawingWidth = 0.3f;

    public Plane drawingPlane;
    
    public Texture2D leafTexture;
    public Color stemColor;

    public GameObject meshObject;
    public Material leafMaterial;
    public GameObject nextButton;

    private bool leafCreated = false;
    
    // Use this for initialization
    void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = drawingWidth;

        /*
        Vector3 screenMidBottom = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * (0.5f - stemWidth), 0, zDist));
        Vector3 screenMidBottom2 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * (0.5f + stemWidth), 0, zDist));
        Vector3 screenMidMid = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * (0.5f - stemWidth), Screen.height * 0.4f, zDist));
        endPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * (0.5f + stemWidth), Screen.height * 0.4f, zDist));
        */

        float xScale = transform.localScale.x;
        float zScale = transform.localScale.z;

        Vector3 midBottomLeft = transform.position + new Vector3(-stemWidth * xScale, lineHeight, -zScale / 2);
        Vector3 midBottomRight = transform.position + new Vector3(stemWidth * xScale, lineHeight, -zScale / 2);
        Vector3 midMidLeft = transform.position + new Vector3(-stemWidth * xScale, lineHeight, -zScale / 2 + 0.4f * zScale / 2);
        endPoint = transform.position + new Vector3(stemWidth * xScale, lineHeight, -zScale / 2 + 0.4f * zScale / 2);
        points.Add(endPoint);
        points.Add(midBottomRight);
        points.Add(midBottomLeft);
        points.Add(midMidLeft);
	}
	
	// Update is called once per frame
	void Update () {
        if (drawingStartDelay > 0) {
            drawingStartDelay -= Time.deltaTime;
        }
        else if ((Input.GetMouseButton(0) || Input.touchCount > 0) && !leafCreated) {
            Vector2 inputPos = (Input.touchCount > 0) ? Input.GetTouch(0).position : new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Ray ray = Camera.main.ScreenPointToRay(inputPos);
            RaycastHit hit;
            Vector3 drawPos = Vector3.zero;

            if (Physics.Raycast(ray, out hit)) {
                drawPos = ray.GetPoint(hit.distance);
                drawPos.y += lineHeight;
                drawPos /= transform.localScale.x;
                if ((drawPos - endPoint).magnitude <= endSnapDistance && points.Count > 5) {
                    leafCreated = true;
                    finalizeLeaf();
                    points.Add(endPoint);
                }
                else if (!pointToClose(drawPos) && !isOverlapping(drawPos)) {
                    points.Add(drawPos); 
                }
            }
        }
        lineRenderer.SetVertexCount(points.Count);
        lineRenderer.SetPositions(points.ToArray());

	}

    private void finalizeLeaf() {
        LeafGenerator lg = new LeafGenerator();
        Mesh leaf = lg.generateLeafMesh(points.ToArray());
        GameObject newLeaf = Instantiate(meshObject);
        newLeaf.transform.localScale = transform.localScale;
        //  leaf = reverseNormals(leaf);
        newLeaf.GetComponent<MeshFilter>().mesh = leaf;
        newLeaf.GetComponent<MeshCollider>().sharedMesh = leaf;
        newLeaf.transform.position = new Vector3(0,5,0);
        createLeafImage(200, 200);
        Destroy(newLeaf);
        Instantiate(nextButton, new Vector3(30, 1, -30), Quaternion.identity);
    }

    public void createLeafImage(int height, int width) {
        Vector3 scale = transform.localScale;
        float xStep = scale.x / (float)(width);
        float zStep = scale.z / (float)(height);

        Texture2D texture = new Texture2D(height,width);
        float heightRatio = leafTexture.height / (height*1f);
        float widthRatio = leafTexture.width / (width*1f);

        for (int y = 0; y <= height; y++) {
            for (int x = 0; x <= width; x++) {
                RaycastHit hit;
                Vector3 from = new Vector3((-scale.x/2 + xStep * x) * scale.x, 10, (-scale.z/2 + zStep*y) * scale.z);
                Vector3 to = from;
                to.y = 0;
                if (Physics.Raycast(from, to - from, out hit)) {
                    if (hit.transform.name == "Leaf(Clone)") {
                        if (x >= (0.5f - stemWidth*1.2f) * width && x < (0.5f + stemWidth * 1.2f) * width && y <= 0.4 * height) {
                            texture.SetPixel(x, y, stemColor);
                        } else {
                            texture.SetPixel(x, y, leafTexture.GetPixel((int)(x/1f * widthRatio), (int)(y / 1f * heightRatio)));
                        }
                    }
                    else {
                        texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                    }
                } else {
                    texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }

        texture.Apply();
        Color32[] pixels = texture.GetPixels32();
        pixels = rotateMatrix(pixels, texture.width, texture.height);
        texture.SetPixels32(pixels);
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.persistentDataPath + "/Leaf.png", bytes);
        leafMaterial.mainTexture = texture;
    }

    private Color32[] rotateMatrix(Color32[] tex, int wid, int hi) {
        Color32[] ret = new Color32[wid * hi];

        for (int y = 0; y < hi; ++y) {
            for (int x = 0; x < wid; ++x) {
                ret[y + (wid - 1 - x) * hi] = tex[x + y * wid];
            }
        }

        return ret;
    }

    private bool isOverlapping(Vector3 pos) {
        Vector3 from = points[points.Count - 1];
        for (int i = 0; i < points.Count-2; ++i) {
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            if (Mathf.Max(from.x,pos.x) >= Mathf.Min(p1.x,p2.x) && Mathf.Max(p1.x,p2.x) >= Mathf.Min(from.x,pos.x)
                && Mathf.Max(from.z,pos.z) >= Mathf.Min(p1.z,p2.z) && Mathf.Max(p1.z,p2.z) >= Mathf.Min(from.z,pos.z)) {
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
