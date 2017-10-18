using UnityEngine;
using System.Collections;

public class EnemyPlane : MonoBehaviour {

    private GameObject cam;
    private EnemyController enemyCon;

    // Use this for initialization
    void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        enemyCon = GetComponentInParent<EnemyController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("Bullet")) {
            Destroy(other.gameObject);
            enemyCon.currentHealth -= 0.8f;
        } else if (other.tag.Equals("Blimp")) {
            enemyCon.Death();
        } else if (other.tag.Equals("Cannonball")) {
            enemyCon.currentHealth -= 20;
        } else if (other.tag.Equals("Player")) {
            enemyCon.currentHealth -= 5;
        } else if (other.tag.Equals("Parts")){
            enemyCon.currentHealth += 5;
            Destroy(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("Proximity")){
            Destroy(enemyCon.gameObject);
            cam.GetComponent<GameController>().currentEnemies -= 1;
        }
    }
}
