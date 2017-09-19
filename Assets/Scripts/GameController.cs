using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

    public bool gameOver = false;
    public int currentBlimps,currentEnemies,numBlimps = 0, numEnemies = 10, score = 0, kills = 0, highScore, highKills;
    public Text scoreText, highScoreText;
    public GameObject enemy, blimp, player;

    private GameObject thisPlayer, thisEnemy;
    private Vector3 offset;

	// Use this for initialization

    void Awake () {
        Screen.orientation = ScreenOrientation.Landscape;
    }

    void Start() {
        scoreText.text = "Score: ";
        highScore = PlayerPrefs.GetInt("Highscore", 0);
        highKills = PlayerPrefs.GetInt("Highkills", 0);
        highScoreText.text = "High Score: " + highScore + "\t\tKills: " + highKills;
        currentEnemies = 0;
        currentBlimps = 0;
        thisPlayer = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Spawn();
        offset = transform.position - thisPlayer.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "Score: " + score + "\t\tKills: " + kills;
        if (score > highScore) {
            highScore = score;
            highScoreText.text = "High Score: " + highScore + "\t\tKills: " + highKills;
            PlayerPrefs.SetInt("Highscore", highScore);
            PlayerPrefs.Save();
        }
        if (kills > highKills) {
            highKills = kills;
            highScoreText.text = "High Score: " + highScore + "\t\tKills: " + highKills;
            PlayerPrefs.SetInt("Highkills", highKills);
            PlayerPrefs.Save();
        }
        if (thisPlayer != null)
            transform.position = thisPlayer.transform.position + offset;
        Spawn();
        if (gameOver) {
            StartCoroutine(Restart());
        }
    }

    Vector3 RandomCircle(Vector3 center, float radius) {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    void Spawn() {
        Vector3 center;
        if (thisPlayer != null)
            center = thisPlayer.transform.position;
        else
            center = new Vector3(0, 0, 0);
        float radius;
        while (currentEnemies < numEnemies) {
            radius = Random.Range(20.0f, 40.0f); //max of range must always be lower than radius of enemyProximity collider in player
            Vector3 pos = RandomCircle(center, radius);
            Quaternion enemyRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Random.rotation.z, Quaternion.identity.w);
            Instantiate(enemy, pos, enemyRot);
            currentEnemies++;
        }
        while (currentBlimps < numBlimps) {
            radius = Random.Range(10.0f, 25.0f);
            Vector3 pos = RandomCircle(center, radius);
            Quaternion blimpRot = new Quaternion(Quaternion.identity.x, Quaternion.identity.y, Random.rotation.z, Quaternion.identity.w);
            Instantiate(blimp, pos, blimpRot);
            currentBlimps++;
        }
    }

    IEnumerator Restart() {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Main");
    }
}
