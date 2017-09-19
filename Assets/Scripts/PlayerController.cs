using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    public int maxHealth, currentParts, maxParts;
    public float currentHealth, currentSpeed, maxSpeed, minSpeed, bulletSpeed, turnRate;
    public Slider healthBar, partsBar;
    public GameObject bullet, leftBulletSpawn, rightBulletSpawn;
    public GameController gameCon;

    private bool fire = false, stop = false;
    private float midSpeed;
    private Rigidbody2D rb;
    private Vector3 lookVec;
    private Quaternion lookAt;

    // Use this for initialization
    void Start () {
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
        if (currentHealth > 0) {
            /*For test builds only*/
            fire = Input.GetKey(KeyCode.Space);
            /*For mobile builds only*/
            //fire = CrossPlatformInputManager.GetButton("FireReloadButton");

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
        lookVec = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"), 1000);

        if (lookVec.x != 0 && lookVec.y != 0) {
            lookAt = Quaternion.LookRotation(lookVec, Vector3.back);
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
        } else if (currentSpeed > midSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 0.995f);
            if (currentSpeed < midSpeed)
                currentSpeed = midSpeed;
        } else if (currentSpeed < midSpeed) {
            currentSpeed = Mathf.Pow(currentSpeed, 1.005f);
            if (currentSpeed > midSpeed)
                currentSpeed = midSpeed;
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
        Destroy(thisLeftBullet, 0.25f);
        Destroy(thisRightBullet, 0.25f);
        yield return new WaitForSeconds(0.01f);
        stop = false;
    }

    IEnumerator Repair ()  {
        while((currentHealth < maxHealth) && (currentParts > 0) && !fire && !stop) {
            stop = true;
            currentHealth += 2;
            healthBar.value = currentHealth;
            currentParts -= 1;
            partsBar.value = currentParts;

            /*For test builds only*/
            fire = Input.GetKey(KeyCode.Space);
            /*For mobile builds only*/
            //fire = CrossPlatformInputManager.GetButton("FireReloadButton");

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
