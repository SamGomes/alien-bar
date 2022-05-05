using System;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneTrainingFunctionalities: MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI trainingTimeDisplay;
    public Button restartButton;
    public void Start()
    {
        scoreDisplay.text = "Final Score: "+ GameGlobals.Score;
        trainingTimeDisplay.text = "Training Time (s): "+ Math.Round(GameGlobals.PlayingTime, 3);
        
        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("StartScene");
        });
    }
}

