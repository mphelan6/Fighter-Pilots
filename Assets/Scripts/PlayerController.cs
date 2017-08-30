using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    public int maxHealth, currentParts, maxParts;
    public float currentHealth, speed, bulletSpeed, turnRate;
    public Slider healthBar, partsBar;
    public GameObject bullet, leftBulletSpawn, rightBulletSpawn;

    private bool fire = false, stop = false;
    private Rigidbody2D rb;
    private Vector3 lookVec;
    private Quaternion lookAt;

    // Use this for initialization
    void Start () {
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
        healthBar.value = currentHealth;
        currentParts = 0;
        partsBar.value = currentParts;
    }

    void Update()  {
        if (currentHealth > 0) {
            /*For test builds only*/
            //fire = Input.GetKey(KeyCode.Space);

            /*For mobile builds only*/
            fire = CrossPlatformInputManager.GetButton("FireReloadButton");

            if (fire && !stop) {
                StartCoroutine(Fire(lookVec));
            }

            if ((maxHealth >= currentHealth) && (currentParts > 0) && !fire) {
                StartCoroutine(Repair());
            }
        } else {
            SceneManager.LoadScene("Main");
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        lookVec = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"), 1000);

        if (lookVec.x != 0 && lookVec.y != 0) {
            lookAt = Quaternion.LookRotation(lookVec, Vector3.back);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnRate * Time.deltaTime);
        }
        rb.AddForce(transform.up * speed);
    }

    IEnumerator Fire (Vector3 look) {
        stop = true;
        GameObject thisLeftBullet = Instantiate(bullet, leftBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        GameObject thisRightBullet = Instantiate(bullet, rightBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        thisLeftBullet.transform.rotation = transform.rotation;
        thisRightBullet.transform.rotation = transform.rotation;
        thisLeftBullet.GetComponent<Rigidbody2D>().AddForce(transform.up * bulletSpeed);
        thisRightBullet.GetComponent<Rigidbody2D>().AddForce(transform.up * bulletSpeed);
        Destroy(thisLeftBullet, 1.5f);
        Destroy(thisRightBullet, 1.5f);
        yield return new WaitForSeconds(0.05f);
        stop = false;
    }

    IEnumerator Repair ()  {
        while((currentHealth < maxHealth) && (currentParts > 0) && !fire && !stop) {
            stop = true;
            currentHealth += 1;
            healthBar.value = currentHealth;
            currentParts -= 1;
            partsBar.value = currentParts;

            /*For test builds only*/
            //fire = Input.GetKey(KeyCode.Space);
            /*For mobile builds only*/
            fire = CrossPlatformInputManager.GetButton("FireReloadButton");

            if (fire) {
                stop = false;
                break;
            } else {
                yield return new WaitForSeconds(0.2f);
            }
            stop = false;
        }
    }
}
