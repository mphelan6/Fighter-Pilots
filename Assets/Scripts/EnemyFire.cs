using UnityEngine;
using System.Collections;

public class EnemyFire : MonoBehaviour {
    public bool fire = false;
    public float bulletSpeed;
    public GameObject bullet, leftBulletSpawn, rightBulletSpawn;

    private bool stop = false;
    private Vector3 force1, force2;
    private GameObject parent;
    private EnemyController enemyCon;

    // Use this for initialization
    void Start() {
        parent = transform.parent.gameObject;
        enemyCon = parent.GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update() {
        if (fire && !stop) {
            StartCoroutine(Fire());
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("Player")) {
            fire = true;
        } else if (other.tag.Equals("Blimp")) {
            enemyCon.avoid = true;
            enemyCon.otherWaypoints = other.gameObject.GetComponentsInChildren<Transform>();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag.Equals("Player")) {
            fire = false;
        } else if (other.tag.Equals("Blimp")) {
            enemyCon.avoid = false;
        }
    }

    IEnumerator Fire() {
        stop = true;
        GameObject thisLeftBullet = Instantiate(bullet, leftBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        GameObject thisRightBullet = Instantiate(bullet, rightBulletSpawn.transform.position, Quaternion.identity) as GameObject;
        thisLeftBullet.transform.rotation = leftBulletSpawn.transform.rotation;
        thisRightBullet.transform.rotation = rightBulletSpawn.transform.rotation;
        force1 = thisLeftBullet.transform.up * bulletSpeed;
        force2 = thisRightBullet.transform.up * bulletSpeed;
        thisLeftBullet.GetComponent<Rigidbody>().AddForce(force1.x, force1.y, 0);
        thisRightBullet.GetComponent<Rigidbody>().AddForce(force2.x, force2.y, 0);
        Destroy(thisLeftBullet, 0.25f);
        Destroy(thisRightBullet, 0.25f);
        yield return new WaitForSeconds(0.005f);
        stop = false;
    }
}
