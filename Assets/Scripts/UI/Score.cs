using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private int
        _maxScore,
        _currentScore;

    [SerializeField]
    private float
        _canvasFadeSpeed = 0.5f,
        _blinkSpeed = 1,
        _blinkStrength = 0.5f,
        _blinkFadeSpeed = 0.1f;

    [SerializeField]
    private CanvasGroup _canvas;

    [SerializeField]
    private Shader _scoreShader;

    [SerializeField]
    private Color _color;

    [SerializeField]
    private UnityEngine.UI.Text _scoreText;

    private Material _mat;

    private OnAction _updater;

    private bool _quitting = false;

    private void Awake()
    {
        Menu.OnPlay += ShowCanvas;
        Menu.OnStop += HideCanvas;

        ServiceLocator.Provide(this);
        _currentScore = 0;

        Menu.OnPlay += ResetScore;

        _mat = new Material(_scoreShader);
        _scoreText.material = _mat;
    }

    private void Start()
    {
        _canvas.alpha = 0;
        _scoreText.text = _currentScore + "/" + _maxScore;

        _mat.SetColor("_color", _color);
        _mat.SetFloat("_on", 1);
        _mat.SetFloat("_timer", 1);

    }

    private void Update()
    {
        if (_updater != null)
            _updater();

        _color.a = (_color.a - (_color.a - 1) / _blinkFadeSpeed) * _canvas.alpha;
        _mat.SetColor("_color", _color); 
    }

    public void RegisterScorePoint()
    {
        _maxScore++;
    }

    public void AddPoint()
    {
        if (_quitting)
            return;

        _currentScore++;
        _scoreText.text = _currentScore + "/" + _maxScore;

        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        float timer = 0;
        float ratio = 1 / _blinkSpeed;

        yield return null;

        while (timer < _blinkSpeed)
        {
            _color.a += Mathf.Cos(timer * ratio * (Mathf.PI / 2)) * _blinkStrength;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void ResetScore()
    {
        _currentScore = 0;
        _scoreText.text = "0/" + _maxScore;
    }

    private void ShowCanvas()
    {
        _updater -= HideCanvas;
        _updater -= ShowCanvas; _updater += ShowCanvas;

        _canvas.alpha += Time.deltaTime * _canvasFadeSpeed;

        if (_canvas.alpha >= 1)
        {
            _canvas.alpha = 1;
            _updater -= ShowCanvas;
        }
    }

    private void HideCanvas()
    {
        _updater -= ShowCanvas;
        _updater -= HideCanvas; _updater += HideCanvas;

        _canvas.alpha -= Time.deltaTime * _canvasFadeSpeed;

        if (_canvas.alpha <= 0)
        {
            _canvas.alpha = 0;
            _updater -= HideCanvas;
        }
    }

    private void OnApplicationQuit()
    {
        _quitting = true;
    }
}
