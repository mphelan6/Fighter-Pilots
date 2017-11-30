using UnityEngine;
using System.Collections;

public class SpawnCheck : MonoBehaviour {

    public bool isEmpty;

	// Use this for initialization
	void Awake () {
        isEmpty = true;
    }

    void OnTriggerStay2D(Collider2D other) {
        isEmpty = false;
    }
}
