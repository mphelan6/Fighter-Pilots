using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    public GameObject playerCamera;
    public GameObject canvas;
    public GameObject eventSystem;

    public void Start() {
        if (eventSystem == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> eventSystem Reference. Please set it up in GameObject 'Game Manager'", this);
        } else {
            PhotonNetwork.Instantiate(eventSystem.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        }

        if (canvas == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> canvas Reference. Please set it up in GameObject 'Game Manager'", this);
        } else {
            PhotonNetwork.Instantiate(canvas.name, new Vector3(232f, 205f, 0f), Quaternion.identity, 0);
        }

        if (playerCamera == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerCamera Reference. Please set it up in GameObject 'Game Manager'", this);
        } else {
            PhotonNetwork.Instantiate(playerCamera.name, new Vector3(0f, 0f, -10f), Quaternion.identity, 0);
        }
    }

    // Called when the local player left the room. We need to load the launcher scene.
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            LeaveRoom();
        }
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene(0);
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    void LoadArena() {
        if (!PhotonNetwork.isMasterClient) {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.Log("PhotonNetwork : Loading Level... ");
        PhotonNetwork.LoadLevel("Main");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer other) {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting
        if (PhotonNetwork.isMasterClient) {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            LoadArena();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other) {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects
        if (PhotonNetwork.isMasterClient) {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            LeaveRoom();
        }
    }
}
