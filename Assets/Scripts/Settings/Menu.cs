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

    [SerializeField]
    private MenuItem[] _items;

    private ObjectSafe _objectSafe;
    private Score      _score;

    private void Awake()
    {
        ServiceLocator.Provide(this);
    }

    void Start ()
    {
        _objectSafe = ServiceLocator.Locate<ObjectSafe>();
        _score      = ServiceLocator.Locate<Score>();

        _objectSafe.Start();

        foreach (MenuItem item in _items)
            item.Selected = false;

        _items[System.Convert.ToInt32(_menuState)].Selected = true;
	}
	
	void Update ()
    {
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
                    Play();

                if (_menuState == MenuState.Quit)
                    Quit();
            }
        }

        if (_gameState == GameState.Game)
            if (Input.GetKeyDown(KeyCode.Escape))
                ReturnToMenu();
    }

    private void Play()
    {
        _gameState = GameState.Game;
        _objectSafe.Spawn();
    }

    private void ReturnToMenu()
    {
        _gameState = GameState.Menu;
        _score.ResetScore();
        _objectSafe.Delete();
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
        _items[System.Convert.ToInt32(_menuState)].Selected = false;

        _menuState += change;
        int num = System.Convert.ToInt32(_menuState);
        int max = System.Enum.GetNames(typeof(MenuState)).Length - 1;

        if (num < 0)
            _menuState += max + 1;

        if (num > max)
            _menuState -= max + 1;

        _items[System.Convert.ToInt32(_menuState)].Selected = true;
    }

    public GameState GameState
    {
        get { return _gameState; }
    }
}
