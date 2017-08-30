using UnityEngine;
using System.Collections;

public class EnemyFire : MonoBehaviour {
    public float bulletSpeed;
    public GameObject bullet, leftBulletSpawn, rightBulletSpawn;

    private bool stop = false, fire = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        if (fire && !stop) {
            StartCoroutine(Fire());
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            fire = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            fire = false;
        }
    }

    IEnumerator Fire() {
        stop = true;
        GameObject thisLeftBullet = Instantiate(bullet, leftBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        GameObject thisRightBullet = Instantiate(bullet, rightBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        thisLeftBullet.transform.rotation = leftBulletSpawn.transform.rotation;
        thisRightBullet.transform.rotation = rightBulletSpawn.transform.rotation;
        thisLeftBullet.GetComponent<Rigidbody2D>().AddForce(transform.up * -bulletSpeed);
        thisRightBullet.GetComponent<Rigidbody2D>().AddForce(transform.up * -bulletSpeed);
        Destroy(thisLeftBullet, 1.5f);
        Destroy(thisRightBullet, 1.5f);
        yield return new WaitForSeconds(0.05f);
        stop = false;
    }
}
