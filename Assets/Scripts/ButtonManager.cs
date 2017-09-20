using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    int index = 0;

    [SerializeField]
    private Button PlayIA;
    [SerializeField]
    private Button HighscoreIA;
    [SerializeField]
    private Button ControlsIA;
    [SerializeField]
    private Button ExitIA;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            index--;
            if (index < 0)
            {
                index = 3;
            }
                Debug.Log(index);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index++;
            if (index > 3)
            {
                index = 0;
            }
                Debug.Log(index);
        }

        switch (index)
        {
            case 0:
                PlayIA.gameObject.SetActive(true);
                SetInactive(PlayIA);
                if (Input.GetKeyDown(KeyCode.I))
                {
                    Play();
                }
                break;
            case 1:
                HighscoreIA.gameObject.SetActive(true);
                SetInactive(HighscoreIA);
                if (Input.GetKeyDown(KeyCode.I))
                {
                    Highscores();
                }
                break;
            case 2:
                ControlsIA.gameObject.SetActive(true);
                SetInactive(ControlsIA);
                if (Input.GetKeyDown(KeyCode.I))
                {
                    Controls();
                }
                break;
            case 3:
                ExitIA.gameObject.SetActive(true);
                SetInactive(ExitIA);
                if (Input.GetKeyDown(KeyCode.I))
                {
                    ExitGame();
                }
                break;
        }
    }

    public void SwapScene(string SceneToSwap)
    {
        SceneManager.LoadScene(SceneToSwap);
    }

    private void Play()
    {
        SwapScene("Scene-Etienne2");
    }

    private void Highscores()
    {
        SwapScene("Highscores");
    }

    private void Controls()
    {
        SwapScene("Controls");
    }

    private void SetInactive(Button pbutton)
    {
        PlayIA.gameObject.SetActive(true);
        HighscoreIA.gameObject.SetActive(true);
        ControlsIA.gameObject.SetActive(true);
        ExitIA.gameObject.SetActive(true);

        pbutton.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
