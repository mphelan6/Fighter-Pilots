using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : Photon.MonoBehaviour {

    public bool PC = false; 
    public int maxHealth, currentParts, maxParts;
    public float currentHealth, currentSpeed, maxSpeed, minSpeed, bulletSpeed, turnRate;
    public Slider healthBar, partsBar;
    public GameObject bullet, leftBulletSpawn, rightBulletSpawn;
    public GameController gameCon;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    private bool fire = false, stop = false;
    private float midSpeed;
    private Rigidbody2D rb;
    private Vector3 lookVec;
    private Quaternion lookAt;
    private GameObject controls;

    private void Awake() {
        if (photonView.isMine) {
            LocalPlayerInstance = gameObject;
        }
    }

    // Use this for initialization
    void Start() {
        if (PC) {
            controls = GameObject.FindGameObjectWithTag("Controls");
            controls.SetActive(false);
        } else {
            controls = GameObject.FindGameObjectWithTag("Controls");
            controls.SetActive(true);
        }
        gameCon = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameController>();
        rb = GetComponent<Rigidbody2D>();
        Slider[] sliders = FindObjectsOfType<Slider>();
        if (sliders[0].tag.Equals("Health Bar")) {
            healthBar = sliders[0];
            partsBar = sliders[1];
        } else {
            partsBar = sliders[0];
            healthBar = sliders[1];
        }
        currentHealth = maxHealth;
        midSpeed = (maxSpeed + minSpeed) / 2f;
        currentSpeed = midSpeed;
        healthBar.value = currentHealth;
        currentParts = 0;
        partsBar.value = currentParts;
    }

    void Update()  {
        if (photonView.isMine == false && PhotonNetwork.connected == true) {
            return;
        }
        if (currentHealth > 0) {
            if (PC) {
                fire = Input.GetKey(KeyCode.Mouse0); //For PC builds only
            } else {
                fire = CrossPlatformInputManager.GetButton("FireReloadButton"); //For mobile builds only
            }

            if (fire && !stop) {
                StartCoroutine(Fire(lookVec));
            }

            if ((maxHealth >= currentHealth) && (currentParts > 0) && !fire) {
                StartCoroutine(Repair());
            }
        } else {
            Death();
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (PC) {
            lookVec = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000);
            lookVec.x -= Screen.width/2;
            lookVec.y -= Screen.height/2;
        } else {
            lookVec = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"), 1000);
        } 
        if (lookVec.x != 0 && lookVec.y != 0) {
            lookAt = Quaternion.LookRotation(lookVec, Vector3.back);
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
        } else if (currentSpeed < maxSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 1.01f);
            if (currentSpeed > maxSpeed)
                currentSpeed = maxSpeed;
        }
        rb.AddForce(transform.up * currentSpeed);
    }

    IEnumerator Fire (Vector3 look) {
        stop = true;
        GameObject thisLeftBullet = Instantiate(bullet, leftBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        GameObject thisRightBullet = Instantiate(bullet, rightBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        thisLeftBullet.transform.rotation = transform.rotation;
        thisRightBullet.transform.rotation = transform.rotation;
        thisLeftBullet.GetComponent<Rigidbody2D>().AddForce(thisLeftBullet.transform.up * bulletSpeed);
        thisRightBullet.GetComponent<Rigidbody2D>().AddForce(thisRightBullet.transform.up * bulletSpeed);
        yield return new WaitForSeconds(0.005f);
        stop = false;
    }

    IEnumerator Repair ()  {
        while((currentHealth < maxHealth) && (currentParts > 0) && !fire && !stop) {
            stop = true;
            currentHealth += 2;
            healthBar.value = currentHealth;
            currentParts -= 1;
            partsBar.value = currentParts;

            if (PC) {
                fire = Input.GetKey(KeyCode.Mouse0); //For PC builds only
            } else {
                fire = CrossPlatformInputManager.GetButton("FireReloadButton"); //For mobile builds only
            }

            if (fire) {
                stop = false;
                break;
            } else {
                yield return new WaitForSeconds(0.2f);
            }
            stop = false;
        }
    }

    void Death() {
        gameCon.gameOver = true;
        Destroy(gameObject);
    }
}
