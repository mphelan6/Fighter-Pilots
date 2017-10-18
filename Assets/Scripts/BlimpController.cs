using UnityEngine;
using System.Collections;

public class BlimpController : MonoBehaviour {

    //Still need to implement behavior in enemies so that they avoid crashing into blimps

    public float currentHealth, speed, turnRate, patrolRad;
    public GameObject parts;

    private bool waypointWait = false;
    private int maxHealth;
    private float radius;
    private Vector3 center, temp, look, spawnPos, wayPoint;
    private Quaternion rot, lookAt;
    private Rigidbody2D rb;

    private GameObject cam;

    // Use this for initialization
    void Start() {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        rb = GetComponent<Rigidbody2D>();
        spawnPos = transform.position;
    }

    void Update() {
        //implement avoidance & attacks
        if (currentHealth <= 0) {
            Death();
            cam.GetComponent<GameController>().score += 200;
        }
    }
	
	void FixedUpdate() {
        Patrol();
    }

    void Patrol() {
        if (!waypointWait) {
            StartCoroutine(NewWaypoint());
        }
        temp = transform.position - wayPoint;
        look = new Vector3(temp.x, temp.y, 1000);
        lookAt = Quaternion.LookRotation(look, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        rb.AddForce(transform.up * speed);
    }

    void Death() {
        Vector3 temp = transform.position;
        Destroy(gameObject);
        cam.GetComponent<GameController>().currentBlimps -= 1;
        Instantiate(parts, temp, Quaternion.identity);
    }

    Vector3 RandomCircle(Vector3 center, float radius) {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    IEnumerator NewWaypoint() {
        waypointWait = true;
        yield return new WaitForSeconds(Random.Range(0f, 10f));
        radius = Random.Range(0.0f, patrolRad);
        wayPoint = RandomCircle(spawnPos, radius);
        waypointWait = false;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player") || other.tag.Equals("Enemy")) {
            currentHealth -= 12.5f;
        } else if (other.tag.Equals("Bullet") || other.tag.Equals("Enemy Bullet")) {
            currentHealth -= 0.25f;
        } else if (other.tag.Equals("Blimp")) {
            currentHealth -= 100;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("Proximity")) {
            Destroy(gameObject);
            cam.GetComponent<GameController>().currentBlimps -= 1;
        }
    }
}
