using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {

    public bool entered = false, exited = true;

    private int diffLvl;
    private int EASY = 1, MEDIUM = 2, HARD = 3;

    private CircleCollider2D patrol;
    private GameObject parent;

	// Use this for initialization
	void Start () {
        parent = transform.parent.gameObject;
        patrol = GetComponent<CircleCollider2D>();
        diffLvl = GetComponentInParent<EnemyController>().diffLvl;
        if (diffLvl == EASY) {
            patrol.radius = 10f;
        } else if (diffLvl == MEDIUM) {
            patrol.radius = 12.5f;
        } else if (diffLvl == HARD) {
            patrol.radius = 15f;
        }
	}

    void Update() {
        transform.position = parent.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Player") && !entered) {
            entered = true;
            exited = false;
            patrol.radius *= 2;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("Player") && !exited) {
            exited = true;
            entered = false;
            patrol.radius /= 2;
        }
    }
}
