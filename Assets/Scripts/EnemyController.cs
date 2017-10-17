using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public bool avoid;
    public int diffLvl;
    public float currentHealth, currentSpeed;
    public Sprite easyEnemy, mediumEnemy, hardEnemy;
    public Transform[] otherWaypoints;
    public GameObject parts;

    private bool entered, exited;
    private int maxHealth, easyRate, mediumRate, hardRate;

    private int EASY = 1, MEDIUM = 2, HARD = 3;

    private bool stop = false, waypointWait = false;
    private float maxSpeed, minSpeed, turnRate, patrolRad, radius, lookWait;
    private Vector3 center, temp, look, spawnPos, wayPoint;
    private Quaternion rot, lookAt;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private GameObject player, cam;

    void Awake() {
        avoid = false;
        enemyAnim = GetComponentInChildren<Animator>();
        easyRate = 50; mediumRate = 85; hardRate = 100;
        Difficulty();
    }

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        spawnPos = transform.position;
    }

    void Update() {
        entered = GetComponentInChildren<EnemyPatrol>().entered;
        exited = GetComponentInChildren<EnemyPatrol>().exited;
        if (currentHealth <= 0) {
            Death();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (avoid) { //when avoiding other vehicles
            Avoid();
        } else if (entered) { //when player is in sight
            if (!stop)
                StartCoroutine(LookAt());
            rb.AddForce(transform.up * currentSpeed);
        } else if (exited) { //when player is not in sight
            Patrol();
        }
    }

    void Difficulty() { //Sets values for enemies based on the difficulty level assigned to them
        int temp = Random.Range(1, 100);
        if (temp <= easyRate) {
            diffLvl = EASY;
            enemyAnim.runtimeAnimatorController = Resources.Load("Easy Enemy") as RuntimeAnimatorController;
            maxSpeed = 150; minSpeed = 60; currentSpeed = maxSpeed; turnRate = 370; lookWait = 0.03f; patrolRad = 10; maxHealth = 50; currentHealth = maxHealth;
        } else if (temp > easyRate && temp <= mediumRate) {
            diffLvl = MEDIUM;
            enemyAnim.runtimeAnimatorController = Resources.Load("Medium Enemy") as RuntimeAnimatorController;
            maxSpeed = 170; minSpeed = 65; currentSpeed = maxSpeed; turnRate = 380; lookWait = 0.02f; patrolRad = 15; maxHealth = 65; currentHealth = maxHealth;
        } else if (temp > mediumRate && temp <= hardRate) {
            diffLvl = HARD;
            enemyAnim.runtimeAnimatorController = Resources.Load("Hard Enemy") as RuntimeAnimatorController;
            maxSpeed = 190; minSpeed = 75; currentSpeed = maxSpeed; turnRate = 390; lookWait = 0.01f; patrolRad = 20; maxHealth = 80; currentHealth = maxHealth;
        }
    }

    void Avoid() { //avoid blimps and other enemies
        Vector3 viewPos = GetComponentInChildren<EnemyFire>().gameObject.transform.position;
        float dis1 = 0, dis2 = 0, dis3 = 0, dis4 = 0;
        if (otherWaypoints[1] != null && otherWaypoints[2] != null && otherWaypoints[3] != null && otherWaypoints[4] != null) {
            dis1 = Vector2.Distance(viewPos, otherWaypoints[1].position);
            dis2 = Vector2.Distance(viewPos, otherWaypoints[2].position);
            dis3 = Vector2.Distance(viewPos, otherWaypoints[3].position);
            dis4 = Vector2.Distance(viewPos, otherWaypoints[4].position);

            if (dis1 <= dis2 && dis1 <= dis3 && dis1 <= dis4) {
                wayPoint = otherWaypoints[1].position;
            } else if (dis2 <= dis1 && dis2 <= dis3 && dis2 <= dis4) {
                wayPoint = otherWaypoints[2].position;
            } else if (dis3 <= dis2 && dis3 <= dis1 && dis3 <= dis4) {
                wayPoint = otherWaypoints[3].position;
            } else if (dis4 <= dis2 && dis4 <= dis3 && dis4 <= dis1) {
                wayPoint = otherWaypoints[4].position;
            }
        }

        temp = transform.position - wayPoint;
        look = new Vector3(temp.x, temp.y, 1000);
        lookAt = Quaternion.LookRotation(look, Vector3.forward);
        float angle2 = Quaternion.Angle(lookAt, transform.rotation);
        if (angle2 >= 5.0f && currentSpeed >= minSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 0.99f); //think about changing turnrate here in the same fashion
            if (currentSpeed < minSpeed)
                currentSpeed = minSpeed;
        } else if (angle2 < 5.0f && currentSpeed < maxSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 1.01f);
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        rb.AddForce(transform.up * currentSpeed);
    }

    void Patrol() { //fly around in circles, partrolling an area
        if (!waypointWait) {
            StartCoroutine(NewWaypoint());
        }
        temp = transform.position - wayPoint;
        look = new Vector3(temp.x, temp.y, 1000);
        lookAt = Quaternion.LookRotation(look, Vector3.forward);
        float angle = Quaternion.Angle(lookAt, transform.rotation);
        if (angle >= 5.0f && currentSpeed >= minSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 0.99f); //think about changing turnrate here in the same fashion
            if (currentSpeed < minSpeed)
                currentSpeed = minSpeed;
        } else if (angle < 5.0f && currentSpeed < maxSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 1.01f);
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        rb.AddForce(transform.up * currentSpeed);
    }

    IEnumerator LookAt() { //track the player's movements to chase them
        stop = true;
        yield return new WaitForSeconds(lookWait);
        if (player != null) {
            center = player.transform.position;
        } else {
            GetComponentInChildren<EnemyPatrol>().enabled = false;
            GetComponentInChildren<EnemyFire>().enabled = false;
        }
        temp = transform.position - center;
        look = new Vector3(temp.x, temp.y, 1000);
        lookAt = Quaternion.LookRotation(look, Vector3.forward);
        float angle = Quaternion.Angle(lookAt, transform.rotation);
        if (angle >= 5.0f && currentSpeed >= minSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 0.99f); //think about changing turnrate here in the same fashion
            if (currentSpeed < minSpeed)
                currentSpeed = minSpeed;
        } else if (angle < 5.0f && currentSpeed < maxSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 1.01f);
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        stop = false;
    }

    public void Death() {
        Vector3 temp = transform.position;
        Destroy(gameObject);
        Instantiate(parts, temp, Quaternion.identity);
        if (diffLvl == EASY) {
            cam.GetComponent<GameController>().score += 25;
        } else if (diffLvl == MEDIUM) {
            cam.GetComponent<GameController>().score += 50;
        } else if (diffLvl == HARD) {
            cam.GetComponent<GameController>().score += 100;
        }
        //scales difficulty of newly instantiated enemies as the player kills more enemies
        easyRate = -2;
        mediumRate = -1;
        cam.GetComponent<GameController>().currentEnemies -= 1;
    }

    //helper methods
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
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        radius = Random.Range(0.0f, patrolRad);
        wayPoint = RandomCircle(spawnPos, radius);
        waypointWait = false;
    }
}
