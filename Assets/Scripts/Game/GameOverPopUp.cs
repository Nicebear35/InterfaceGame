using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameOverPopUp : MonoBehaviour
{
    public GameObject GameOver;
    public GameObject LosePopUp;
    public GameObject NewBestScorePopUp;

    private void Start()
    {
        GameOver.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver(bool newBestScore)
    {
        GameOver.SetActive(true);
        LosePopUp.SetActive(false);
        NewBestScorePopUp.SetActive(true);
    }
}
