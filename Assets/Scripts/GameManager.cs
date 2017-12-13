using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Photon.PunBehaviour {

    public GameObject player;

    public bool gameOver = false;
    public int currentBlimps, currentEnemies, numBlimps, numEnemies, score = 0, highScore;
    public Text highScoreText;
    public GameObject enemy, blimp, spawnCheck, cam, cam1, cam2, ScoreTextPrefab;

    private GameObject firstPlayer, secondPlayer, scoreText;

    public void Start() {
        if (player == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerCamera Reference. Please set it up in GameObject 'Game Manager'", this);
        } else {
            if (PhotonNetwork.isMasterClient) {
                firstPlayer = PhotonNetwork.Instantiate(player.name, new Vector3(-2.5f, 0, 0), Quaternion.identity, 0);
                cam.GetComponent<CameraController>().player = firstPlayer;
                Instantiate(cam, new Vector3(-2.5f, 0, -10), Quaternion.identity);              
            } else {
                secondPlayer = PhotonNetwork.Instantiate(player.name, new Vector3(2.5f, 0, 0), Quaternion.identity, 0);
                cam.GetComponent<CameraController>().player = secondPlayer;
                Instantiate(cam, new Vector3(2.5f, 0, -10), Quaternion.identity);
            }
        }

        if (ScoreTextPrefab == null) {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> ScoreUIPrefab reference on GameManager Prefab.", this);
        } else {
            if (PhotonNetwork.isMasterClient) {
                scoreText = PhotonNetwork.Instantiate(ScoreTextPrefab.name, new Vector3(100, 390, 0), Quaternion.identity, 0);
                scoreText.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
        }

        /*
        scoreText.text = "Score: ";
        highScore = PlayerPrefs.GetInt("Highscore", 0);
        highScoreText.text = "High Score: " + highScore;*/
        currentEnemies = 0;
        currentBlimps = 0;

        if (PhotonNetwork.isMasterClient)
            Spawn();
    }

    // Called when the local player left the room. We need to load the launcher scene.
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Q) || gameOver) {
            LeaveRoom();
        }

        /*
        scoreText.text = "Score: " + score;
        if (score > highScore) {
            highScore = score;
            highScoreText.text = "High Score: " + highScore;
            PlayerPrefs.SetInt("Highscore", highScore);
            PlayerPrefs.Save();
        }*/

        if (PhotonNetwork.isMasterClient) {
            if (currentEnemies < numEnemies || currentBlimps < numBlimps)
                Spawn();
        }

        if (firstPlayer == null && secondPlayer == null) {
            gameOver = true;
        }
    }

    void Spawn() {
        if (firstPlayer != null && secondPlayer != null) {
            Vector3 firstPlayerPos, secondPlayerPos;
            Vector3 check;
            while (currentEnemies < numEnemies) {
                if (currentEnemies % 2 == 0) { // even number of enemies
                    firstPlayerPos = firstPlayer.transform.position;
                    check = RandomCircle(firstPlayerPos, UnityEngine.Random.Range(20f, 40f));
                    GameObject thisCheck = PhotonNetwork.Instantiate(spawnCheck.name, check, Quaternion.identity, 0) as GameObject;
                    if (thisCheck.GetComponent<SpawnCheck>().isEmpty) {
                        Quaternion enemyRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, UnityEngine.Random.rotation.z, Quaternion.identity.w);
                        PhotonNetwork.Instantiate(enemy.name, check, enemyRot, 0);
                        currentEnemies++;
                    }
                    if (photonView.isMine) {
                        PhotonNetwork.Destroy(thisCheck);
                    }
                } else if (currentEnemies % 2 == 1) { // odd number of enemies
                    secondPlayerPos = secondPlayer.transform.position;
                    check = RandomCircle(secondPlayerPos, UnityEngine.Random.Range(20f, 40f));
                    GameObject thisCheck = PhotonNetwork.Instantiate(spawnCheck.name, check, Quaternion.identity, 0) as GameObject;
                    if (thisCheck.GetComponent<SpawnCheck>().isEmpty) {
                        Quaternion enemyRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, UnityEngine.Random.rotation.z, Quaternion.identity.w);
                        PhotonNetwork.Instantiate(enemy.name, check, enemyRot, 0);
                        currentEnemies++;
                    }
                    if (photonView.isMine) {
                        PhotonNetwork.Destroy(thisCheck);
                    }
                }
            }
            while (currentBlimps < numBlimps) {
                if (currentBlimps % 2 == 0) { // even number of blimps
                    firstPlayerPos = firstPlayer.transform.position;
                    check = RandomCircle(firstPlayerPos, UnityEngine.Random.Range(20f, 40f));
                    GameObject thisCheck = PhotonNetwork.Instantiate(spawnCheck.name, check, Quaternion.identity, 0) as GameObject;
                    if (thisCheck.GetComponent<SpawnCheck>().isEmpty) {
                        Quaternion blimpRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, UnityEngine.Random.rotation.z, Quaternion.identity.w);
                        PhotonNetwork.Instantiate(blimp.name, check, blimpRot, 0);
                        currentBlimps++;
                    }
                    if (photonView.isMine) {
                        PhotonNetwork.Destroy(thisCheck);
                    }
                } else if (currentBlimps % 2 ==1) { // odd number of blimps
                    secondPlayerPos = secondPlayer.transform.position;
                    check = RandomCircle(secondPlayerPos, UnityEngine.Random.Range(20f, 40f));
                    GameObject thisCheck = PhotonNetwork.Instantiate(spawnCheck.name, check, Quaternion.identity, 0) as GameObject;
                    if (thisCheck.GetComponent<SpawnCheck>().isEmpty) {
                        Quaternion blimpRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, UnityEngine.Random.rotation.z, Quaternion.identity.w);
                        PhotonNetwork.Instantiate(blimp.name, check, blimpRot, 0);
                        currentBlimps++;
                    }
                    if (photonView.isMine) {
                        PhotonNetwork.Destroy(thisCheck);
                    }
                }
            }
        }
    }

    //helper functions
    Vector3 RandomCircle(Vector3 center, float radius) {
        float ang = UnityEngine.Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
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
