using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscorePanelScript : MonoBehaviour {

    [SerializeField]
    private int index;

    [SerializeField]
    private Text ScoreText;

    [SerializeField]
    private Text NameText;

    void Start ()
    {
        UpdateList();
	}

    void UpdateList()
    {
        int pScore;
        string pName;
        HighscoresScript.ReturnDataByID(index, out pScore, out pName);

        ScoreText.text = pScore.ToString();
        NameText.text = pName.ToString();
    }
}
