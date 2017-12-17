using UnityEngine;
using System.Collections;

public class SpawnCheck : MonoBehaviour {

    public bool isEmpty;

	// Use this for initialization
	void Awake () {
        isEmpty = true;
    }

    void OnTriggerStay(Collider other) {
        isEmpty = false;
    }
}
