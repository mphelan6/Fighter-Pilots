using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int diffLvl;
    public float currentHealth;
    public Sprite easyEnemy, mediumEnemy, hardEnemy;
    public GameObject parts;

    private bool entered, exited;
    private int maxHealth;

    private int EASY = 1, MEDIUM = 2, HARD = 3;

    private bool stop = false, waypointWait = false;
    private float speed, turnRate, patrolRad, radius, lookWait;
    private Vector3 center, temp, look, spawnPos, wayPoint;
    private Quaternion rot;
    private Quaternion lookAt;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private GameObject player, cam;

    void Awake() {
        enemyAnim = GetComponentInChildren<Animator>();
        Difficulty();
    }

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        
        spawnPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
	}

    void Update() {
        entered = GetComponentInChildren<EnemyPatrol>().entered;
        exited = GetComponentInChildren<EnemyPatrol>().exited;
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
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (exited) {
            Patrol();
        } else if (entered) {
            Chase();
            if (!stop)
                StartCoroutine(LookAt());
        }
    }

    void Difficulty() {
        int temp = Random.Range(1, 100);
        if (temp <= 50) {
            diffLvl = EASY;
            enemyAnim.runtimeAnimatorController = Resources.Load("Easy Enemy") as RuntimeAnimatorController;
            speed = 110;
            turnRate = 170;
            lookWait = 0.03f;
            patrolRad = 10;
            maxHealth = 50;
            currentHealth = maxHealth;
        } else if (temp > 50 && temp <= 85) {
            diffLvl = MEDIUM;
            enemyAnim.runtimeAnimatorController = Resources.Load("Medium Enemy") as RuntimeAnimatorController;
            speed = 115;
            turnRate = 180;
            lookWait = 0.02f;
            patrolRad = 15;
            maxHealth = 75;
            currentHealth = maxHealth;
        } else if (temp > 86 && temp <= 100) {
            diffLvl = HARD;
            enemyAnim.runtimeAnimatorController = Resources.Load("Hard Enemy") as RuntimeAnimatorController;
            speed = 120;
            turnRate = 190;
            lookWait = 0.01f;
            patrolRad = 20;
            maxHealth = 100;
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        rb.AddForce(transform.up * speed);
    }

    IEnumerator LookAt() {
        stop = true;
        yield return new WaitForSeconds(lookWait);
        center = player.transform.position;
        temp = transform.position - center;
        look = new Vector3(temp.x, temp.y, 1000);
        lookAt = Quaternion.LookRotation(look, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        stop = false;
    }

    void Chase() {
        rb.AddForce(transform.up * speed);
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
