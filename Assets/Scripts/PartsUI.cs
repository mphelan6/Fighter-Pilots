﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PartsUI : MonoBehaviour {

    [Tooltip("UI Text for Parts")]
    public Text PartsText;

    [Tooltip("UI Slider to display Player's Parts")]
    public Slider PartsSlider;

    private PlayerController _target;

    void Awake() {
        GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Reflect the Player Health
        if (PartsSlider != null)
        {
            PartsSlider.value = _target.currentParts;
        }
    }

    public void SetTarget(PlayerController target) {
        if (target == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayerController target for PartsUI.SetTarget.", this);
            return;
        }
        _target = target;
    }
}