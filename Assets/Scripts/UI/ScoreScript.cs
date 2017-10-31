using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    public static int currentScore;

    [SerializeField]
        private Text scoreAmount;

	void Update ()
    {
        UpdateScore();

        if (Input.GetKeyDown(KeyCode.Home))
        {
            AddScore(50);
        }
    }

    private void UpdateScore()
    {
        scoreAmount.text = currentScore.ToString();
    }

    private void AddScore(int pBaseScoreAmount)
    {
        currentScore += pBaseScoreAmount;
    }
}
