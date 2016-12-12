using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Inspiration from: http://wiki.unity3d.com/index.php?title=Flocking 
 */

public class BoidManager : MonoBehaviour {

    public int boidCreatureCount = 20;
    public float minSpeed = 1.0f;
    public float maxSpeed = 5.0f;
    public float minNeighbourDistance = 0.2f;
    public float randomness = 1.0f;

    private float lastTime = 0;
    private int lastNeighbourUpdate = 0;
    public float neighbourUpdateTime = 2.0f;

    public Vector3 tendingPlace;


    public GameObject[] boids;

    public Vector3 boidSize;
    public GameObject boidCreature;

    private Vector3 boidCenter;
    private Vector3 boidSpeed;

	// Use this for initialization
	void Start () {
        boids = new GameObject[boidCreatureCount];
        for (int i = 0; i < boidCreatureCount; i++) {
            boids[i] = Instantiate(boidCreature);
            boids[i].transform.parent = transform;
            float xPos = Random.Range(-boidSize.x, boidSize.x);
            float yPos = Random.Range(-boidSize.y, boidSize.y);
            float zPos = Random.Range(-boidSize.z, boidSize.z);
            boids[i].transform.localPosition = new Vector3(xPos, yPos, zPos);
            boids[i].GetComponent<BoidController>().setBoidManager(gameObject);
            boids[i].transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 center = Vector3.zero;
        Vector3 totalVelocity = Vector3.zero;
        foreach (GameObject boid in boids) {
            center += boid.transform.localPosition;
            totalVelocity += boid.GetComponent<Rigidbody>().velocity;
        }
        boidCenter = center / boidCreatureCount;
        boidSpeed = totalVelocity / boidCreatureCount;
        updateBoidNeighbours();
	}

    public Vector3 getBoidCenter(Vector3 boidPosition) {
        return ((boidCenter*boidCreatureCount - boidPosition)/(boidCreatureCount-1));
    }

    public Vector3 getBoidSpeed() {
        return boidSpeed;
    }

    private void updateBoidNeighbours() {
        int amountToUpdate = Mathf.Max((int)(Time.deltaTime * neighbourUpdateTime), 1);
        for (int b = lastNeighbourUpdate; b < lastNeighbourUpdate + amountToUpdate && b < boids.Length; b++) {
            GameObject boid = boids[b];
            BoidController bC = boid.GetComponent<BoidController>();
            List<GameObject> otherBoids = new List<GameObject>(boids);
            int k = 0;

            for (int i = 0; i < bC.neighboursCount; i++) {
                GameObject closest = closestBoid(boid, otherBoids);
                bC.neighbours[k] = closest;
                k++;
                otherBoids.Remove(closest);
            }
        }
        lastNeighbourUpdate += amountToUpdate;
        lastNeighbourUpdate = (lastNeighbourUpdate > boids.Length) ? 0 : lastNeighbourUpdate;
    }

    private GameObject closestBoid(GameObject boid, List<GameObject> otherBoids) {
        Vector3 boidPosition = boid.transform.localPosition;
        GameObject closestBoid = null;

        for (int i = 0; i < otherBoids.Count; i++) {
            GameObject other = otherBoids[i];
            if (boid != other) {
                if (closestBoid == null || (other.transform.localPosition - boidPosition).magnitude < (closestBoid.transform.localPosition - boidPosition).magnitude) {
                    closestBoid = other;
                }
            }
        }

        return closestBoid;
    }

}
