using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class GameController : Photon.PunBehaviour {

    [SerializeField]
    public float xPos = -2.5f;

    public bool gameOver = false;
    public int currentBlimps, currentEnemies, numBlimps, numEnemies, score = 0, highScore;
    public Text scoreText, highScoreText;
    public GameObject enemy, blimp, player, spawnCheck;

    private GameObject thisPlayer, thisEnemy;
    private Vector3 offset;

	// Use this for initialization

    void Awake () {
        Screen.orientation = ScreenOrientation.Landscape;
    }

    void Start() {
        scoreText.text = "Score: ";
        highScore = PlayerPrefs.GetInt("Highscore", 0);
        highScoreText.text = "High Score: " + highScore;
        currentEnemies = 0;
        currentBlimps = 0;
        /*if (photonView.isMine || PhotonNetwork.connected == false) { //for testing offline
            thisPlayer = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        }*/
        if (PlayerController.LocalPlayerInstance == null) {
            if (photonView.isMine) {
                thisPlayer = PhotonNetwork.Instantiate(player.name, new Vector3(xPos, 0, 0), Quaternion.identity, 0) as GameObject;
                offset = transform.position - thisPlayer.transform.position;
            }
        }
        
        if (PhotonNetwork.isMasterClient) {
            Spawn();
        }
    }

    // Update is called once per frame
    void Update() {
        scoreText.text = "Score: " + score;
        if (score > highScore) {
            highScore = score;
            highScoreText.text = "High Score: " + highScore;
            PlayerPrefs.SetInt("Highscore", highScore);
            PlayerPrefs.Save();
        }

        if (thisPlayer != null && (photonView.isMine || PhotonNetwork.connected == false))
            transform.position = thisPlayer.transform.position + offset;
        if (PhotonNetwork.isMasterClient) {
            if (currentEnemies < numEnemies || currentBlimps < numBlimps)
                Spawn();
        }
        if (gameOver)
            StartCoroutine(Restart());
    }

    void Spawn() {
        if (thisPlayer != null) {
            Vector3 playerPos;
            Vector3 check;
            while (currentEnemies < numEnemies) {
                playerPos = thisPlayer.transform.position;
                check = RandomCircle(playerPos, Random.Range(20f, 40f));
                GameObject thisCheck = Instantiate(spawnCheck, check, Quaternion.identity) as GameObject;
                if (thisCheck.GetComponent<SpawnCheck>().isEmpty) {
                    Quaternion enemyRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Random.rotation.z, Quaternion.identity.w);
                    Instantiate(enemy, check, enemyRot);
                    currentEnemies++;
                }
                Destroy(thisCheck);
            }
            while (currentBlimps < numBlimps) {
                playerPos = thisPlayer.transform.position;
                check = RandomCircle(playerPos, Random.Range(20f, 40f));
                GameObject thisCheck = Instantiate(spawnCheck, check, Quaternion.identity) as GameObject;
                if (thisCheck.GetComponent<SpawnCheck>().isEmpty) {
                    Quaternion blimpRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Random.rotation.z, Quaternion.identity.w);
                    Instantiate(blimp, check, blimpRot);
                    currentBlimps++;
                }
                Destroy(thisCheck);
            }
        }
    }

    IEnumerator Restart() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Main");
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("Bullet") || other.tag.Equals("Enemy Bullet") || other.tag.Equals("Cannonball")) {
            Destroy(other.gameObject);
        }
    }

    //helper functions
    Vector3 RandomCircle(Vector3 center, float radius) {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(xPos);
        } else {
            xPos = (float)stream.ReceiveNext();
        }
    }
}