using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

    [Tooltip("UI Text to display Player's Name")]
    public Text PlayerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    public Slider PlayerHealthSlider;

    [Tooltip("Pixel offset from the player target")]
    public Vector3 ScreenOffset = new Vector3(0f, 10f, 0f);

    private PlayerController _target;
    Transform _targetTransform;
    Vector3 _targetPosition;


    void Awake() {
        GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (_target == null) {
            Destroy(gameObject);
            return;
        }

        // Reflect the Player Health
        if (PlayerHealthSlider != null) {
            PlayerHealthSlider.value = _target.currentHealth;
        }
    }

    void LateUpdate() {
        // #Critical
        // Follow the Target GameObject on screen.
        if (_targetTransform != null) {
            _targetPosition = _targetTransform.position;
            transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + ScreenOffset;
        }
    }

    public void SetTarget(PlayerController target) {
        if (target == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayerController target for HealthUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency
        _target = target;
        _targetTransform = target.GetComponent<Transform>();
        if (PlayerNameText != null) {
            PlayerNameText.text = _target.photonView.owner.NickName;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(PlayerHealthSlider.value);
        } else {
            PlayerHealthSlider.value = (float)stream.ReceiveNext();
        }
    }
}
