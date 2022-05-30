using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneTrainingFunctionalities: MonoBehaviour
{
    public TextMeshProUGUI resultsDisplay;
    public Button restartButton;
    public void Start()
    {
        resultsDisplay.text = "Final Score: " +
                            GameGlobals.Score;
        
        resultsDisplay.text += "\nDelivered Orders: " +
                             JsonConvert.SerializeObject(GameGlobals.NumDeliveredOrdersByLevel) +
                             "\n";
        resultsDisplay.text += "Failed Orders: " +
                             JsonConvert.SerializeObject(GameGlobals.NumFailedOrdersByLevel) +
                            "\n"; 
        
        resultsDisplay.text += "Training Time (s): "+ Math.Round(GameGlobals.PlayingTime, 3);

        if (GameGlobals.CurrGameMode == GameMode.TRAINING)
        {
            Dictionary<string, string> logEntry = new Dictionary<string, string>()
            {
                {"GameId", GameGlobals.ExperimentId},
                {"PlayerId", GameGlobals.PlayerId},
                {"GameMode", GameGlobals.CurrGameMode.ToString()},
                {"AttemptId", GameGlobals.AttemptId.ToString()},
                {"OrderDifficulty", GameGlobals.GameConfigs.OrderDifficulty.ToString()},
                {"Score", GameGlobals.Score.ToString()},
                {"NumDeliveredOrdersByLevel", JsonConvert.SerializeObject(GameGlobals.NumDeliveredOrdersByLevel)},
                {"NumFailedOrdersByLevel", JsonConvert.SerializeObject(GameGlobals.NumFailedOrdersByLevel)},
                {"PlayingTime", GameGlobals.PlayingTime.ToString()}
            };
            StartCoroutine(GameGlobals.LogManager.WriteToLog("AlienBarExperiment/TrainingAttempts",
                GameGlobals.ExperimentId + "_" + GameGlobals.PlayerId, logEntry));
        }

        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("StartScene");
        });
    }
}

