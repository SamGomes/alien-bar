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
    public TextMeshProUGUI titleDisplay;
    public TextMeshProUGUI resultsDisplay;
    public Button restartButton;
    public void Start()
    {
        titleDisplay.text = GameGlobals.CurrGameMode + " Over!";
        
        
        resultsDisplay.text = "Score: " +
                            GameGlobals.Score;
        
        resultsDisplay.text += "\nDelivered Orders: " +
                             JsonConvert.SerializeObject(GameGlobals.NumDeliveredOrdersByLevel) +
                             "\n";
        resultsDisplay.text += "Failed Orders: " +
                             JsonConvert.SerializeObject(GameGlobals.NumFailedOrdersByLevel) +
                            "\n"; 
        
        resultsDisplay.text += "Time Spent (s): "+ Math.Round(GameGlobals.SessionTimeSpent, 3);

        //log results
        if (GameGlobals.CurrGameMode == GameMode.TUTORIAL)
        {
            Dictionary<string, string> logEntry = new Dictionary<string, string>()
            {
                {"GameId", GameGlobals.ExperimentId},
                {"PlayerId", GameGlobals.PlayerId},
                {"GameMode", GameGlobals.CurrGameMode.ToString()},
                {"OrderDifficulty", GameGlobals.GameConfigs.OrderDifficulty.ToString()},
                {"Score", GameGlobals.Score.ToString()},
                
                
                {"NumDeliveredLvl1Recipes", GameGlobals.NumDeliveredOrdersByLevel[0].ToString()},
                {"NumDeliveredLvl2Recipes", GameGlobals.NumDeliveredOrdersByLevel[1].ToString()},
                {"NumDeliveredLvl3Recipes", GameGlobals.NumDeliveredOrdersByLevel[2].ToString()},
                {"NumDeliveredLvl4Recipes", GameGlobals.NumDeliveredOrdersByLevel[3].ToString()},
                {"NumDeliveredLvl5Recipes", GameGlobals.NumDeliveredOrdersByLevel[4].ToString()},
                
                {"NumFailedLvl1Recipes", GameGlobals.NumFailedOrdersByLevel[0].ToString()},
                {"NumFailedLvl2Recipes", GameGlobals.NumFailedOrdersByLevel[1].ToString()},
                {"NumFailedLvl3Recipes", GameGlobals.NumFailedOrdersByLevel[2].ToString()},
                {"NumFailedLvl4Recipes", GameGlobals.NumFailedOrdersByLevel[3].ToString()},
                {"NumFailedLvl5Recipes", GameGlobals.NumFailedOrdersByLevel[4].ToString()},

                {"TimeSpent", GameGlobals.SessionTimeSpent.ToString()}
            };
            StartCoroutine(GameGlobals.LogManager.WriteToLog("AlienBarExperiment/TUTORIAL/Results/",
                GameGlobals.ExperimentId + "_" + GameGlobals.PlayerId, logEntry, false));
            
        }else 
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
                
                {"NumDeliveredLvl1Recipes", GameGlobals.NumDeliveredOrdersByLevel[0].ToString()},
                {"NumDeliveredLvl2Recipes", GameGlobals.NumDeliveredOrdersByLevel[1].ToString()},
                {"NumDeliveredLvl3Recipes", GameGlobals.NumDeliveredOrdersByLevel[2].ToString()},
                {"NumDeliveredLvl4Recipes", GameGlobals.NumDeliveredOrdersByLevel[3].ToString()},
                {"NumDeliveredLvl5Recipes", GameGlobals.NumDeliveredOrdersByLevel[4].ToString()},
                
                {"NumFailedLvl1Recipes", GameGlobals.NumFailedOrdersByLevel[0].ToString()},
                {"NumFailedLvl2Recipes", GameGlobals.NumFailedOrdersByLevel[1].ToString()},
                {"NumFailedLvl3Recipes", GameGlobals.NumFailedOrdersByLevel[2].ToString()},
                {"NumFailedLvl4Recipes", GameGlobals.NumFailedOrdersByLevel[3].ToString()},
                {"NumFailedLvl5Recipes", GameGlobals.NumFailedOrdersByLevel[4].ToString()},
                
                {"TimeSpent", GameGlobals.SessionTimeSpent.ToString()}
            };
            StartCoroutine(GameGlobals.LogManager.WriteToLog("AlienBarExperiment/TRAINING/Attempts/",
                GameGlobals.ExperimentId + "_" + GameGlobals.PlayerId, logEntry, false));
        }

        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("StartScene");
        });
    }
}

