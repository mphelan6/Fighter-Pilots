using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    [SerializeField]
    public float currentSpeed;
    [SerializeField]
    private Vector3 center, wayPoint;
    [SerializeField]
    private float wait;
    [SerializeField]
    private int diffnum;

    public bool avoid;
    public int diffLvl;
    public float currentHealth;
    public Sprite easyEnemy, mediumEnemy, hardEnemy;
    public Transform[] otherWaypoints;
    public GameObject parts;
    public GameManager manager;

    public int EASY = 1, MEDIUM = 2, HARD = 3;

    private bool entered, exited;
    private int maxHealth, easyRate, mediumRate, hardRate;

    private bool stop = false, waypointWait = false;
    private float maxSpeed, minSpeed, turnRate, patrolRad, radius, lookWait;
    private Vector3 temp, look, spawnPos, dist1, dist2;
    private Quaternion rot, lookAt;
    private Rigidbody rb;
    private GameObject[] players;

    void Awake() {
        avoid = false;
        easyRate = 50; mediumRate = 85; hardRate = 100;
        Difficulty();
    }

    // Use this for initialization
    void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");
        rb = GetComponent<Rigidbody>();
        spawnPos = transform.position;
    }

    void Update() {
        entered = GetComponentInChildren<EnemyPatrol>().entered;
        exited = GetComponentInChildren<EnemyPatrol>().exited;
        if (currentHealth <= 0 && PhotonNetwork.isMasterClient) {
            Killed();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (avoid) { //when avoiding other vehicles
            //Avoid();
        } else if (entered) { //when player is in sight
            if (!stop)
                StartCoroutine(LookAt());
            Vector3 force = transform.up * currentSpeed;
            rb.AddForce(force.x, force.y, 0);
        } else if (exited) { //when player is not in sight
            Patrol();
        }
    }

    void Difficulty() { //Sets values for enemies based on the difficulty level assigned to them
        if (PhotonNetwork.isMasterClient) {
            diffnum = Random.Range(1, 100);
        }
        if (diffnum <= easyRate) {
            diffLvl = EASY;
            maxSpeed = 150; minSpeed = 60; currentSpeed = maxSpeed; turnRate = 370; lookWait = 0.03f; patrolRad = 10; maxHealth = 50; currentHealth = maxHealth;
        } else if (diffnum > easyRate && diffnum <= mediumRate) {
            diffLvl = MEDIUM;
            maxSpeed = 170; minSpeed = 65; currentSpeed = maxSpeed; turnRate = 380; lookWait = 0.02f; patrolRad = 15; maxHealth = 65; currentHealth = maxHealth;
        } else if (diffnum > mediumRate && diffnum <= hardRate) {
            diffLvl = HARD;
            maxSpeed = 190; minSpeed = 75; currentSpeed = maxSpeed; turnRate = 390; lookWait = 0.01f; patrolRad = 20; maxHealth = 80; currentHealth = maxHealth;
        }
    }

    /*void Avoid() { //avoid blimps and other enemies
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
        Vector3 force = transform.up * currentSpeed;
        rb.AddForce(force.x, force.y, 0);
    }*/

    void Patrol() { //fly around in circles, partrolling an area
        if (PhotonNetwork.isMasterClient) {
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
            Vector3 force = transform.up * currentSpeed;
            rb.AddForce(force.x, force.y, 0);
        } else if (!PhotonNetwork.isMasterClient) {
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
            Vector3 force = transform.up * currentSpeed;
            rb.AddForce(force.x, force.y, 0);
        }
    }

    IEnumerator LookAt() { //track the player's movements to chase them
        if (PhotonNetwork.isMasterClient) {
            stop = true;
            yield return new WaitForSeconds(lookWait);
            if (players.Length == 2) { 
                if (players[0] != null && players[1] != null) {
                    dist1 = transform.position - players[0].transform.position;
                    dist2 = transform.position - players[1].transform.position;
                    if (dist1.magnitude <= dist2.magnitude) {
                        center = players[0].transform.position;
                    } else if (dist2.magnitude <= dist1.magnitude) {
                        center = players[1].transform.position;
                    }
                } else if (players[0] == null) {
                    center = players[1].transform.position;
                } else if (players[1] == null) {
                    center = players[0].transform.position;
                } else {
                    GetComponentInChildren<EnemyPatrol>().enabled = false;
                    GetComponentInChildren<EnemyFire>().enabled = false;
                }
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
        } else if (!PhotonNetwork.isMasterClient) {
            stop = true;
            yield return new WaitForSeconds(lookWait);
            if (players.Length == 2) { 
                if (players[0] == null && players[1] == null) {
                    GetComponentInChildren<EnemyPatrol>().enabled = false;
                    GetComponentInChildren<EnemyFire>().enabled = false;
                }
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
    }
    public void Killed() {
        Vector3 temp = transform.position;
        PhotonNetwork.Instantiate(parts.name, temp, Quaternion.identity, 0);
        if (diffLvl == EASY) {
            manager.score += 25;
        } else if (diffLvl == MEDIUM) {
            manager.score += 50;
        } else if (diffLvl == HARD) {
            manager.score += 100;
        }
        //scales difficulty of newly instantiated enemies as the player kills more enemies
        easyRate -= 2;
        mediumRate -= 1;
        Destroy(gameObject);
    }

    // Will be called AUTOMATICALLY after Killed() by EnemyPlane.OnTriggerExit()
    public void Death() { 
        manager.currentEnemies -= 1;
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
        if (PhotonNetwork.isMasterClient) {
            wait = Random.Range(0f, 1f);
            radius = Random.Range(0.0f, patrolRad);
            wayPoint = RandomCircle(spawnPos, radius);
        }
        yield return new WaitForSeconds(wait);
        waypointWait = false;
    }

    public void SetTarget(GameManager target)  {
        if (target == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> GameManager target for EnemyController.SetTarget.", this);
            return;
        }
        manager = target;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(diffnum);
            stream.SendNext(center);
            stream.SendNext(wait);
            stream.SendNext(wayPoint);
            stream.SendNext(currentSpeed);
        } else {
            diffnum = (int)stream.ReceiveNext();
            center = (Vector3)stream.ReceiveNext();
            wait = (float)stream.ReceiveNext();
            wayPoint = (Vector3)stream.ReceiveNext();
            currentSpeed = (float)stream.ReceiveNext();
        }
    }
}
