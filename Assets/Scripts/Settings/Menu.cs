using System.Collections;
using UnityEngine;

public delegate void OnAction();


public enum GameState
{
    Menu,
    Game
}

public class Menu : MonoBehaviour
{

    public static OnAction OnPlay;
    public static OnAction OnStop;

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

    private float _hold = 0;

    private void Awake()
    {
        ServiceLocator.Provide(this);
    }

    void Start ()
    {
        ServiceLocator.Locate<ObjectSafe>().Start();

        foreach (MenuItem item in _items)
            item.Selected = false;

        _items[System.Convert.ToInt32(_menuState)].Selected = true;
	}
	
	void Update ()
    {
        float selector = Input.GetAxis("Selector 1") + Input.GetAxis("Selector 2");

        if (_gameState == GameState.Menu)
        {
            if (Input.GetButtonDown("Up") || (selector > 0.9f && _hold == 0))
                ChangeState(1);

            if (Input.GetButtonDown("Down") || (selector < -0.9f && _hold == 0))
                ChangeState(-1);

            if (Input.GetButtonDown("Quit"))
                Quit();

            if (Input.GetButton("Select / Shoot"))
            {
                if (_menuState == MenuState.Play)
                    Play();

                if (_menuState == MenuState.Quit)
                    Quit();
            }
        }

        if (_gameState == GameState.Game)
            if (Input.GetButtonDown("Quit"))
                Stop();

        _hold = selector;
    }

    private void Play()
    {
        _gameState = GameState.Game;
        OnPlay();
    }

    public void Stop()
    {
        _gameState = GameState.Menu;
        OnStop();
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


    public void Stop(float wait)
    {
        StartCoroutine(WaitAndStop(wait));
    }

    private IEnumerator WaitAndStop(float wait)
    {
        yield return new WaitForSeconds(wait);
        Stop();
    }

    public GameState GameState
    {
        get { return _gameState; }
    }
}
