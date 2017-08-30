using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {

    public int currentBlimps,currentEnemies,numBlimps = 0, numEnemies = 10, score = 0, kills = 0, highScore;
    public Text scoreText, highScoreText;
    public GameObject enemy, blimp, player;

    private GameObject thisPlayer, thisEnemy;
    private Vector3 offset;
    private Quaternion blimpRot;

	// Use this for initialization

    void Awake () {
        Screen.orientation = ScreenOrientation.Landscape;
    }

    void Start() {
        scoreText.text = "Score: ";
        highScore = PlayerPrefs.GetInt("Highscore", 0);
        highScoreText.text = "High Score: " + highScore;
        thisPlayer = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        offset = transform.position - thisPlayer.transform.position;
        blimpRot = blimp.transform.rotation;
        currentEnemies = 0;
    }
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "Score: " + score + "\t\tKills: " + kills;
        if (score > highScore) {
            highScore = score;
            highScoreText.text = "High Score: " + highScore;
            PlayerPrefs.SetInt("Highscore", highScore);
            PlayerPrefs.Save();
        }
        transform.position = thisPlayer.transform.position + offset;
        Spawn();
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
        Vector3 center = thisPlayer.transform.position;
        float radius;
        while (currentEnemies < numEnemies) {
            radius = Random.Range(15.0f, 40.0f); //max of range must always be lower than radius of enemyProximity collider in player
            Vector3 pos = RandomCircle(center, radius);
            Vector3 temp = pos - center;
            Vector3 look = new Vector3(temp.x, temp.y, 1000);
            Quaternion rot = Quaternion.LookRotation(look, Vector3.forward);
            Instantiate(enemy, pos, rot);
            currentEnemies++;
        }
        while (currentBlimps < numBlimps) {
            radius = Random.Range(10.0f, 25.0f);
            Vector3 pos = RandomCircle(center, radius);
            Instantiate(blimp, pos, blimpRot);
            currentBlimps++;
        }
    }
}
