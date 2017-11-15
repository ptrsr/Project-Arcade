using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private int
        _maxScore,
        _currentScore;

    private void Awake()
    {
        ServiceLocator.Provide(this);
        _currentScore = 0;
    }

    public void RegisterScorePoint()
    {
        _maxScore++;
    }

    public void AddPoint()
    {
        _currentScore++;
    }

    public void ResetScore()
    {
        _currentScore = 0;
    }
}
