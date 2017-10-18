using UnityEngine;
using System.Collections;

public class PlayerPlane : MonoBehaviour {

    private PlayerController playerCon;

    // Use this for initialization
    void Start () {
        playerCon = GetComponentInParent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Enemy Bullet")) {
            Destroy(other.gameObject);
            playerCon.currentHealth -= 0.5f;
            playerCon.healthBar.value = playerCon.currentHealth;
        } else if (other.tag.Equals("Blimp")) {
            playerCon.currentHealth = 0;
            playerCon.healthBar.value = playerCon.currentHealth;
        } else if (other.tag.Equals("Cannonball")) {
            playerCon.currentHealth -= 30;
        } else if (other.tag.Equals("Enemy")) {
            playerCon.currentHealth -= 2.5f;
            playerCon.healthBar.value = playerCon.currentHealth;
        } else if ((other.tag.Equals("Parts")) && (playerCon.currentParts <= playerCon.maxParts - 10)) {
            Destroy(other.gameObject);
            playerCon.currentParts += 10;
            playerCon.partsBar.value = playerCon.currentParts;
        }
    }
}
