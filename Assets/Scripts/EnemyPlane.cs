using UnityEngine;
using System.Collections;

public class EnemyPlane : Photon.MonoBehaviour {

    private EnemyController enemyCon;

    // Use this for initialization
    void Start () {
        enemyCon = GetComponentInParent<EnemyController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other) {
        if (PhotonNetwork.isMasterClient) {
            if (other.tag.Equals("Bullet")) {
                PhotonNetwork.Destroy(other.gameObject);
                enemyCon.currentHealth -= 1;
            } else if (other.tag.Equals("Blimp")) {
                enemyCon.Death();
                enemyCon.Killed();
            } else if (other.tag.Equals("Cannonball")) {
                PhotonNetwork.Destroy(other.gameObject);
                enemyCon.currentHealth -= 20;
            } else if (other.tag.Equals("Player")) {
                enemyCon.currentHealth -= 5;
            } else if (other.tag.Equals("Parts")) {
                enemyCon.currentHealth += 5;
                PhotonNetwork.Destroy(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag.Equals("Proximity") && PhotonNetwork.isMasterClient) {
            enemyCon.Death();
            PhotonNetwork.Destroy(enemyCon.gameObject);
        }
    }
}
