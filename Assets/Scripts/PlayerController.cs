using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : Photon.PunBehaviour {

    [SerializeField]
    public bool fire = false;
    [SerializeField]
    public float currentHealth;

    public int maxHealth, currentParts, maxParts;
    public float currentSpeed, maxSpeed, minSpeed, bulletSpeed, turnRate;
    public Slider healthBar, partsBar;
    public GameObject bullet, leftBulletSpawn, rightBulletSpawn;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    private bool stop = false;
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
        if (photonView.isMine) {
            rb = GetComponent<Rigidbody2D>();
            /*
            Slider[] sliders = FindObjectsOfType<Slider>();
            if (sliders[0].tag.Equals("Health Bar")) {
                healthBar = sliders[0];
                partsBar = sliders[1];
            } else {
                partsBar = sliders[0];
                healthBar = sliders[1];
            } */
            currentHealth = maxHealth;
            midSpeed = (maxSpeed + minSpeed) / 2f;
            currentSpeed = midSpeed;
            //healthBar.value = currentHealth;
            currentParts = 0;
            //partsBar.value = currentParts;
        } else if (!photonView.isMine) {
            rb = GetComponent<Rigidbody2D>();
            midSpeed = (maxSpeed + minSpeed) / 2f;
            currentSpeed = midSpeed;
        }
    }

    void Update()  {
        if (photonView.isMine) {
            if (currentHealth > 0) {
                fire = Input.GetKey(KeyCode.Mouse0);

                if (fire && !stop) {
                    StartCoroutine(Fire());
                }

                if ((maxHealth >= currentHealth) && (currentParts > 0) && !fire) {
                    StartCoroutine(Repair());
                }
            } else {
                Death();
            }
        } else if (!photonView.isMine) {
            if (fire && !stop) {
                StartCoroutine(Fire());
            } 
        } 
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (photonView.isMine) {
            lookVec = new Vector3(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2, 1000);
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
        } else if (!photonView.isMine) {
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
    }

    IEnumerator Fire () {
        stop = true;
        GameObject thisLeftBullet = Instantiate(bullet, leftBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        GameObject thisRightBullet = Instantiate(bullet, rightBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        thisLeftBullet.transform.rotation = transform.rotation;
        thisRightBullet.transform.rotation = transform.rotation;
        thisLeftBullet.GetComponent<Rigidbody2D>().AddForce(thisLeftBullet.transform.up * bulletSpeed);
        thisRightBullet.GetComponent<Rigidbody2D>().AddForce(thisRightBullet.transform.up * bulletSpeed);
        Destroy(thisLeftBullet, 0.25f);
        Destroy(thisRightBullet, 0.25f);
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

            fire = Input.GetKey(KeyCode.Mouse0);

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
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(fire);
            stream.SendNext(lookVec);
            stream.SendNext(currentSpeed);
        } else {
            fire = (bool) stream.ReceiveNext();
            lookVec = (Vector3) stream.ReceiveNext();
            currentSpeed = (float)stream.ReceiveNext();
        }
    }
}
