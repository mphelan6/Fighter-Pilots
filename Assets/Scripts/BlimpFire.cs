using UnityEngine;
using System.Collections;

public class BlimpFire : MonoBehaviour {
    public bool fire = false;
    public float cannonballSpeed, rotincrement;
    public GameObject cannonball;

    private bool stop = false;
    private Vector3 playerPos;
    private Transform playerTrans;
    private BlimpController blimpCon;

	// Use this for initialization
	void Start () {
        blimpCon = GetComponentInParent<BlimpController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (fire && !stop) {
            StartCoroutine(Fire());
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            fire = true;
            playerTrans = other.gameObject.transform;
            playerPos = playerTrans.position;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            fire = false;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.tag.Equals("Player")) {
            playerPos = playerTrans.position;
            Vector2 direction = playerPos - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotincrement);
        }
    }

        IEnumerator Fire() {
        stop = true;
        GameObject thisCannonball = Instantiate(cannonball, transform.position, Quaternion.identity) as GameObject;
        thisCannonball.transform.rotation = transform.rotation;
        thisCannonball.GetComponent<Rigidbody2D>().AddForce(thisCannonball.transform.up * cannonballSpeed);
        yield return new WaitForSeconds(0.5f);
        stop = false;
    }
}
