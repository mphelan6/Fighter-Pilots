using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {

    public bool entered = false, exited = true;

    private int diffLvl;
    private int EASY = 1, MEDIUM = 2, HARD = 3;

    private SphereCollider patrol;
    private GameObject parent;

	// Use this for initialization
	void Start () {
        parent = transform.parent.gameObject;
        patrol = GetComponent<SphereCollider>();
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

    }

    void OnTriggerEnter(Collider other) {
        if (other.tag.Equals("Player") && !entered) {
            entered = true;
            exited = false;
            if (PhotonNetwork.isMasterClient) {
                patrol.radius *= 2;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag.Equals("Player") && !exited) {
            exited = true;
            entered = false;
            if (PhotonNetwork.isMasterClient) {
                patrol.radius /= 2;
            }
        }
    }
}
