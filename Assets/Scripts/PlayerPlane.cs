using UnityEngine;
using System.Collections;

public class PlayerPlane : MonoBehaviour {

    private PlayerController playerCon;
    private PhotonView thisPV;

    // Use this for initialization
    void Start () {
        playerCon = GetComponentInParent<PlayerController>();
        thisPV = GetComponentInParent<PhotonView>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Enemy Bullet")) {
            if (thisPV.photonView.isMine) {
                PhotonNetwork.Destroy(other.gameObject);
            }
            playerCon.currentHealth -= 0.5f;
        } else if (other.tag.Equals("Blimp")) {
            playerCon.currentHealth = 0;
        } else if (other.tag.Equals("Cannonball")) {
            if (thisPV.photonView.isMine) {
                PhotonNetwork.Destroy(other.gameObject);
            }
            playerCon.currentHealth -= 20;
        } else if (other.tag.Equals("Enemy")) {
            playerCon.currentHealth -= 5f;
        } else if ((other.tag.Equals("Parts")) && (playerCon.currentParts <= playerCon.maxParts - 10)) {
            if (thisPV.photonView.isMine) {
                PhotonNetwork.Destroy(other.gameObject);
            }
            playerCon.currentParts += 10;
        }
    }
}
