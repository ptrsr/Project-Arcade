using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterNameScript : MonoBehaviour {

    [SerializeField]
    private Text _char1;
    [SerializeField]
    private Text _char2;
    [SerializeField]
    private Text _char3;
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private GameObject selection1;
    [SerializeField]
    private GameObject selection2;
    [SerializeField]
    private GameObject selection3;

    private int _char1Value = 65;
    private int _char2Value = 65;
    private int _char3Value = 65;

    private int _charindex = 0;
	
	void Update ()
    {
        _scoreText.text = ScoreScript.currentScore.ToString();

		switch(_charindex)
        {
            case 0:
                _char1Value = SetCharValue(_char1Value);
                var valueToText = System.Convert.ToChar(_char1Value);
                _char1.text = valueToText.ToString();
                SelectChar(selection1);
                Next();
                break;
            case 1:
                _char2Value = SetCharValue(_char2Value);
                var valueToText2 = System.Convert.ToChar(_char2Value);
                _char2.text = valueToText2.ToString();
                SelectChar(selection2);
                Next();
                break;
            case 2:
                _char3Value = SetCharValue(_char3Value);
                var valueToText3 = System.Convert.ToChar(_char3Value);
                _char3.text = valueToText3.ToString();
                SelectChar(selection3);
                Next();
                break;
        }
	}

    void Next()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _charindex++;
            if (_charindex > 2)
            {
                string completeName = _char1.text.ToString() + _char2.text.ToString()  +_char3.text.ToString();
                Debug.Log(completeName);

                HighscoresScript.CreateHighscore();
                HighscoresScript.highscores.CreateContainer();

                HighscoresScript.highscores.LoadAndSort();

                HighscoresScript.AddPlayerHighscore(completeName, ScoreScript.currentScore);

                Debug.Log("Player Added: " + completeName + " With Score: " + ScoreScript.currentScore);

                SceneManager.LoadScene("Main-Menu");
            }
        }
    }

    int SetCharValue(int pCharValue)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            pCharValue--;

            return CheckCharValue(pCharValue);

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            pCharValue++;

            return CheckCharValue(pCharValue);
        }
        else
        {
            return pCharValue;
        }
    }

    int CheckCharValue(int pCharValue)
    {
        if (pCharValue < 65)
        {
            return 90;
        }
        else if (pCharValue > 90)
        {
            return 65;
        }
        else
        {
            return pCharValue;
        }
    }

    void SelectChar(GameObject pChar)
    {
        selection1.gameObject.SetActive(false);
        selection2.gameObject.SetActive(false);
        selection3.gameObject.SetActive(false);
        pChar.gameObject.SetActive(true);
    }
}
