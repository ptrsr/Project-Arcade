using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void SwapSceneButton(string SceneToSwap)
    {
        SceneManager.LoadScene(SceneToSwap);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
