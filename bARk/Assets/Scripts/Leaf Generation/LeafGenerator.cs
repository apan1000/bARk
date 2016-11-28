using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeafGenerator {

	
    public Mesh generateLeafMesh(Vector3[] points) {
        Mesh leaf = new Mesh();

      /*  for (int i = 0; i < points.Length; ++i) {
            points[i].x = -points[i].x;
        }*/

        Vector2[] vertices2D = toVector2(points);

        //Triangulator t = new Triangulator(vertices);

        Triangulator t = new Triangulator(vertices2D);
        int[] indices = t.Triangulate();

        leaf.vertices = points;
        leaf.triangles = indices;
        leaf.RecalculateNormals();
        leaf.RecalculateBounds();

        return leaf;
    }


    private Vector2[] toVector2(Vector3[] points) {
        Vector2[] newVec = new Vector2[points.Length];
        for (int i = 0; i < points.Length; ++i) {
            newVec[i] = new Vector2(points[i].x, points[i].y);
        }
        return newVec;
    }


}
