using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum GameState
{
    Menu,
    Game
}

public class Menu : MonoBehaviour
{
    private enum MenuState
    {
        Play,
        Quit
    }

    [SerializeField]
    private GameState _gameState = GameState.Menu;
    [SerializeField]
    private MenuState _menuState = MenuState.Play;

    private void Awake()
    {
        ServiceLocator.Provide(this);
    }

    void Start ()
    {
        ServiceLocator.Locate<ObjectSafe>().Start();
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.O))
            ServiceLocator.Locate<ObjectSafe>().Delete();

        if (Input.GetKeyDown(KeyCode.P))
            ServiceLocator.Locate<ObjectSafe>().Spawn();

        if (_gameState == GameState.Menu)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                ChangeState(1);

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                ChangeState(-1);

            if (Input.GetKeyDown(KeyCode.Escape))
                Quit();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_menuState == MenuState.Play)
                    _gameState = GameState.Game;

                if (_menuState == MenuState.Quit)
                    Quit();
            }
        }
    }

    private void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    private void ChangeState(int change)
    {
        _menuState += change;
        int num = System.Convert.ToInt32(_menuState);
        int max = System.Enum.GetNames(typeof(MenuState)).Length - 1;

        if (num < 0)
            _menuState += max + 1;

        if (num > max)
            _menuState -= max + 1;
    }

    public GameState GameState
    {
        get { return _gameState; }
    }
}
