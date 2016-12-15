using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour {
    
    public int neighboursCount = 10;
    private BoidManager boidManager;

    public GameObject[] neighbours;
    public float tdist;

    private Rigidbody rigid;

	// Use this for initialization
	void Start () {
        rigid = GetComponent<Rigidbody>();
        neighbours = new GameObject[neighboursCount];
        for (int i = 0; i < neighboursCount; i++) {
            neighbours[i] = null;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rigid.velocity += getAddedVelocity() * Time.deltaTime;
        float speed = rigid.velocity.magnitude;
        if (speed > boidManager.maxSpeed) {
            rigid.velocity = rigid.velocity.normalized * boidManager.maxSpeed;
        } else if (speed < boidManager.minSpeed) {
            rigid.velocity = rigid.velocity.normalized * boidManager.minSpeed;
        }
        transform.forward = rigid.velocity;
    }

    public void setBoidManager(GameObject manager) {
        boidManager = manager.GetComponent<BoidManager>();
    }

    private Vector3 getAddedVelocity() {

        Vector3 randomDir = new Vector3((2*Random.value)-1, (2 * Random.value) - 1, (2 * Random.value) - 1);
        randomDir = randomDir.normalized;

        return (centerVelocity() + averageVelocity() + avoidanceVelocity() + tendToPlace(boidManager.tendingPlace) + 
            avoidancePointVelocity() + cameraAvoidanceVelocity() + randomDir * boidManager.randomness);
    }

    private Vector3 centerVelocity() {
        Vector3 center = boidManager.getBoidCenter(transform.localPosition);
        return (center - transform.localPosition) * 5.0f;
    }

    private Vector3 averageVelocity() {
        Vector3 speed = boidManager.getBoidSpeed();

        return (speed - rigid.velocity) / 4;
    }

    private Vector3 tendToPlace(Vector3 position) {
        return (position - transform.localPosition) * 2;
    }

    private Vector3 avoidanceVelocity() {
        Vector3 c = Vector3.zero;

        foreach (GameObject boid in neighbours) {
            if (boid == null)
                continue;
            Vector3 dif = boid.transform.localPosition - transform.localPosition;
            if (dif.magnitude < boidManager.minNeighbourDistance) {
                c = c - dif;
            }
        }
        return c;
    }

    private Vector3 avoidancePointVelocity() {
        Vector3 avoidancePointDif = transform.localPosition - boidManager.avoidancePoint;
        avoidancePointDif.y = 0;
        if (avoidancePointDif.magnitude <= boidManager.avoidanceDistance) {
            return avoidancePointDif.normalized * 500 / avoidancePointDif.magnitude;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Attempts to steer boid away from the camera.
    /// </summary>
    /// <returns>Velocity equal to direction away from camera if too close, zero vector if not. </returns>
    private Vector3 cameraAvoidanceVelocity() {
        Vector3 dif = transform.localPosition - Camera.main.transform.position;
        tdist = dif.magnitude;
        if (dif.magnitude <= boidManager.cameraAvoidanceDistance) {
            dif.y = 0;
            return dif.normalized * 20;
        }
        return Vector3.zero;
    }

}
