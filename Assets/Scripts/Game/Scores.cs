using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int Score = 0;
}

public class Scores : MonoBehaviour
{
    public Text ScoreText;

    private bool _newBestScore;
    private BestScoreData _bestScore = new BestScoreData();

    private int _currentScore;
    private string _bestScoreKey = "Best score data";

    private void Awake()
    {
        if (BinaryDataStream.IsFileExist(_bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        _bestScore = BinaryDataStream.Read<BestScoreData>(_bestScoreKey);
        yield return new WaitForEndOfFrame();

        Debug.Log("Read best score = " + _bestScore.Score);
    }

    private void Start()
    {
        _newBestScore = false;
        _currentScore = 0;
        DisplayScore();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScore;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScore;
    }

    public void SaveBestScore(bool newBestScore)
    {
        BinaryDataStream.Save<BestScoreData>(_bestScore, _bestScoreKey);
    }

    private void AddScores(int scores)
    {
        _currentScore += scores;

        if (_currentScore > _bestScore.Score)
        {
            _newBestScore = true;
            _bestScore.Score = _currentScore;
        }

        DisplayScore();
    }

    private void DisplayScore()
    {
        ScoreText.text = _currentScore.ToString();
    }
}
