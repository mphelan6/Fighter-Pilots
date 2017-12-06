using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class part_rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(30*Time.deltaTime, 40*Time.deltaTime, 90*Time.deltaTime));
	}
}
