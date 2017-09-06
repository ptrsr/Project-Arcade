using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    public static int currentScore;

    [SerializeField]
        private Text scoreAmount;
    [SerializeField]
        private Text highScoreAmount;

	void Start ()
    {
        highScoreAmount.text = PlayerPrefs.GetInt("HighscoreAmount", 0).ToString();
	}
	

	void Update ()
    {
        UpdateScore();
        UpdateHighscore();

        if (Input.GetKeyDown(KeyCode.Home))
        {
            IncrementScore();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            ResetHighscore();
        }
    }

    private void UpdateScore()
    {
        scoreAmount.text = currentScore.ToString();
    }

    private void UpdateHighscore()
    {
        if(currentScore > PlayerPrefs.GetInt("HighscoreAmount",0))
        {
            PlayerPrefs.SetInt("HighscoreAmount", currentScore);
            highScoreAmount.text = currentScore.ToString();
        }
    }

    private void IncrementScore()
    {
        currentScore += 10;
    }

    public void ResetHighscore()
    {
        PlayerPrefs.SetInt("HighscoreAmount", 0);
        highScoreAmount.text = "0";
        currentScore = 0;
    }
}
