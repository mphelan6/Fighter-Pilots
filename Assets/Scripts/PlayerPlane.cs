using UnityEngine;
using System.Collections;

public class PlayerPlane : MonoBehaviour {

    private GameObject cam;
    private PlayerController playerCon;

    // Use this for initialization
    void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        playerCon = GetComponentInParent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Enemy Bullet")) {
            Destroy(other.gameObject);
            playerCon.currentHealth -= 1;
            playerCon.healthBar.value = playerCon.currentHealth;
        } else if (other.tag.Equals("Blimp")) {
            Destroy(other.gameObject);
            cam.GetComponent<GameController>().currentBlimps -= 1;
            playerCon.currentHealth -= 50;
            playerCon.healthBar.value = playerCon.currentHealth;
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
