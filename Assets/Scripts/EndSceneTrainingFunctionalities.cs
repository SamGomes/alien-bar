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
        titleDisplay.text = GameGlobals.CurrGameMode + " Game Over!";
        
        
//        resultsDisplay.text = "Score: " +
//                            GameGlobals.Score;
        
        resultsDisplay.text = "\nDelivered Orders: " +
                             JsonConvert.SerializeObject(GameGlobals.NumDeliveredOrdersByLevel) +
                             "\n";
        resultsDisplay.text += "Failed Orders: " +
                             JsonConvert.SerializeObject(GameGlobals.NumFailedOrdersByLevel) +
                            "\n"; 
        
        resultsDisplay.text += "Time Spent (s): "+ Math.Round(GameGlobals.SessionTimeSpent, 3);

        //log results
        Dictionary<string, string> logEntry = new Dictionary<string, string>()
        {
            {"ExperimentId", GameGlobals.ExperimentId},
            {"ParticipantId", GameGlobals.ParticipantId},
            {"GameMode", GameGlobals.CurrGameMode.ToString()},
            {"AttemptId", GameGlobals.AttemptId.ToString()},
            {"OrderLevel_AtEnd", GameGlobals.GameConfigs.OrderDifficulty.ToString()},
            {"Score", GameGlobals.Score.ToString()},
            
            
            {"NumDeliveredLvl1Orders", GameGlobals.NumDeliveredOrdersByLevel[0].ToString()},
            {"NumDeliveredLvl2Orders", GameGlobals.NumDeliveredOrdersByLevel[1].ToString()},
            {"NumDeliveredLvl3Orders", GameGlobals.NumDeliveredOrdersByLevel[2].ToString()},
            {"NumDeliveredLvl4Orders", GameGlobals.NumDeliveredOrdersByLevel[3].ToString()},
            {"NumDeliveredLvl5Orders", GameGlobals.NumDeliveredOrdersByLevel[4].ToString()},
            
            {"NumFailedLvl1Orders", GameGlobals.NumFailedOrdersByLevel[0].ToString()},
            {"NumFailedLvl2Orders", GameGlobals.NumFailedOrdersByLevel[1].ToString()},
            {"NumFailedLvl3Orders", GameGlobals.NumFailedOrdersByLevel[2].ToString()},
            {"NumFailedLvl4Orders", GameGlobals.NumFailedOrdersByLevel[3].ToString()},
            {"NumFailedLvl5Orders", GameGlobals.NumFailedOrdersByLevel[4].ToString()},

            
            {"NumDeliveredLvl1Recipes", GameGlobals.NumDeliveredRecipesByLevel[0].ToString()},
            {"NumDeliveredLvl2Recipes", GameGlobals.NumDeliveredRecipesByLevel[1].ToString()},
            {"NumDeliveredLvl3Recipes", GameGlobals.NumDeliveredRecipesByLevel[2].ToString()},
            {"NumDeliveredLvl4Recipes", GameGlobals.NumDeliveredRecipesByLevel[3].ToString()},
            {"NumDeliveredLvl5Recipes", GameGlobals.NumDeliveredRecipesByLevel[4].ToString()},
            
            
            {"TimeSpent", GameGlobals.SessionTimeSpent.ToString()}
        };

        string database = "AlienBarExperiment/ERRORS";
        if (GameGlobals.CurrGameMode == GameMode.TUTORIAL)
        {
            database = "AlienBarExperiment/TUTORIAL/Results/";
        }else if (GameGlobals.CurrGameMode == GameMode.TRAINING)
        {
            database = "AlienBarExperiment/TRAINING/Attempts/";
        }

        StartCoroutine(GameGlobals.LogManager.WriteToLog(
            database,
            GameGlobals.ExperimentId + "_" + GameGlobals.ParticipantId, logEntry, false));
        

        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("StartScene");
        });
    }
}

