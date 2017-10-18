using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

    public bool gameOver = false;
    public int currentBlimps, currentEnemies, numBlimps = 0, numEnemies = 10, score = 0, highScore;
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
        thisPlayer = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        offset = transform.position - thisPlayer.transform.position;
        Spawn();
    }
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "Score: " + score;
        if (score > highScore) {
            highScore = score;
            highScoreText.text = "High Score: " + highScore;
            PlayerPrefs.SetInt("Highscore", highScore);
            PlayerPrefs.Save();
        }

        if (thisPlayer != null)
            transform.position = thisPlayer.transform.position + offset;
        Spawn();
        if (gameOver) {
            StartCoroutine(Restart());
        }
    }

    void Spawn() {
        if (thisPlayer != null) {
            Vector3 playerPos = thisPlayer.transform.position;
            Vector3 check;
            GameObject thisCheck;
            while (currentEnemies < numEnemies) {
                check = RandomCircle(playerPos, Random.Range(20f, 40f));
                thisCheck = Instantiate(spawnCheck, check, Quaternion.identity) as GameObject;
                if (thisCheck.GetComponent<SpawnCheck>().isEmpty) {
                    Quaternion enemyRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Random.rotation.z, Quaternion.identity.w);
                    Instantiate(enemy, check, enemyRot);
                    currentEnemies++;
                }
                Destroy(thisCheck);
            }
            while (currentBlimps < numBlimps) {
                check = RandomCircle(playerPos, Random.Range(20f, 40f));
                thisCheck = Instantiate(spawnCheck, check, Quaternion.identity) as GameObject;
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
}
