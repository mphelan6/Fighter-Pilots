using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int diffLvl;
    public float currentHealth, currentSpeed;
    public Sprite easyEnemy, mediumEnemy, hardEnemy;
    public GameObject parts;

    private bool entered, exited;
    private int maxHealth, easyRate, mediumRate, hardRate;

    private int EASY = 1, MEDIUM = 2, HARD = 3;

    private bool stop = false, waypointWait = false;
    private float midSpeed, maxSpeed, minSpeed, turnRate, patrolRad, radius, lookWait;
    private Vector3 center, temp, look, spawnPos, wayPoint;
    private Quaternion rot;
    private Quaternion lookAt;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private GameObject player, cam;

    void Awake() {
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
        if (GetComponentInChildren<EnemyPatrol>().enabled == true) {
            entered = GetComponentInChildren<EnemyPatrol>().entered;
            exited = GetComponentInChildren<EnemyPatrol>().exited;
        } else {
            entered = false;
            exited = true;
        }
        if (currentHealth <= 0) {
            Death();
            if (diffLvl == EASY) {
                cam.GetComponent<GameController>().score += 25;
            } else if (diffLvl == MEDIUM) {
                cam.GetComponent<GameController>().score += 50;
            } else if (diffLvl == HARD) {
                cam.GetComponent<GameController>().score += 100;
            }
            cam.GetComponent<GameController>().kills += 1;
            //scales difficulty of newly instantiated enemies as the player kills more enemies
            easyRate = -2;
            mediumRate = -1;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (exited) {
            Patrol();
        } else if (entered) {
            rb.AddForce(transform.up * currentSpeed);
            if (!stop)
                StartCoroutine(LookAt());
        }
    }

    void Difficulty() {
        int temp = Random.Range(1, 100);
        if (temp <= easyRate) {
            diffLvl = EASY;
            enemyAnim.runtimeAnimatorController = Resources.Load("Easy Enemy") as RuntimeAnimatorController;
            maxSpeed = 120;
            minSpeed = 70;
            midSpeed = (maxSpeed + minSpeed) / 2f;
            currentSpeed = midSpeed;
            turnRate = 170;
            lookWait = 0.03f;
            patrolRad = 10;
            maxHealth = 50;
            currentHealth = maxHealth;
        } else if (temp > easyRate && temp <= mediumRate) {
            diffLvl = MEDIUM;
            enemyAnim.runtimeAnimatorController = Resources.Load("Medium Enemy") as RuntimeAnimatorController;
            maxSpeed = 140;
            minSpeed = 80;
            midSpeed = (maxSpeed + minSpeed) / 2f;
            currentSpeed = midSpeed;
            turnRate = 180;
            lookWait = 0.02f;
            patrolRad = 15;
            maxHealth = 65;
            currentHealth = maxHealth;
        } else if (temp > mediumRate && temp <= hardRate) {
            diffLvl = HARD;
            enemyAnim.runtimeAnimatorController = Resources.Load("Hard Enemy") as RuntimeAnimatorController;
            maxSpeed = 160;
            minSpeed = 90;
            midSpeed = (maxSpeed + minSpeed) / 2f;
            currentSpeed = midSpeed;
            turnRate = 190;
            lookWait = 0.01f;
            patrolRad = 20;
            maxHealth = 80;
            currentHealth = maxHealth;
        }
    }

    void Patrol() {
        if (!waypointWait) {
            StartCoroutine(NewWaypoint());
        }
        temp = transform.position - wayPoint;
        look = new Vector3(temp.x, temp.y, 1000);
        lookAt = Quaternion.LookRotation(look, Vector3.forward);
        float angle = Quaternion.Angle(lookAt, transform.rotation);
        if (angle >= 10.0f && currentSpeed >= minSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 0.995f);
            if (currentSpeed < minSpeed)
                currentSpeed = minSpeed;
        } else if (angle < 20.0f && currentSpeed <= maxSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 1.005f);
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        rb.AddForce(transform.up * currentSpeed);
    }

    IEnumerator LookAt() {
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
        if (angle >= 10.0f && currentSpeed >= minSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 0.995f);
            if (currentSpeed < minSpeed)
                currentSpeed = minSpeed;
        } else if (angle < 20.0f && currentSpeed <= maxSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 1.005f);
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        stop = false;
    }

    void Death() {
        Vector3 temp = transform.position;
        Destroy(gameObject);
        cam.GetComponent<GameController>().currentEnemies -= 1;
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
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        radius = Random.Range(0.0f, patrolRad);
        wayPoint = RandomCircle(spawnPos, radius);
        waypointWait = false;
    }
}
