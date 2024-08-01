using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneSurvivalFunctionalities: MonoBehaviour
{
    public TextMeshProUGUI resultsDisplay;
    
    public GameObject engQuestionnaire;
    public Button endButton;
    
    private string SerializeOrderList(List<int> orders)
    {
        string ret = "[";
        for (int i = 0; i < orders.Count(); i++)
        {
            ret += orders[i];
            if (i < orders.Count - 1)
                ret += ",";
        }
        ret += "]";
        return ret;
    }
    public void Start()
    {
        
        resultsDisplay.text = "\nDelivered Orders: " +
                              SerializeOrderList(GameGlobals.NumDeliveredOrdersByLevel) +
                               "\n";
        resultsDisplay.text += "Failed Orders: " +
                               SerializeOrderList(GameGlobals.NumFailedOrdersByLevel) +
                               "\n"; 
        
        resultsDisplay.text += "Time Spent(s): "+ Math.Round(GameGlobals.SessionTimeSpent, 3);
        

        
        Dictionary<string, string> logEntry = new Dictionary<string, string>()
        {
            {"ExperimentId", GameGlobals.ExperimentId},
            {"ParticipantId", GameGlobals.ParticipantId},
            {"GameMode", GameGlobals.CurrGameMode.ToString()},
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
        StartCoroutine(GameGlobals.LogManager.WriteToLog("AlienBarExperiment/SURVIVAL/Results/",
            GameGlobals.ExperimentId + "_" + GameGlobals.ParticipantId, logEntry, false));
        
        if (GameGlobals.GameConfigs.IsDemo)
        {
            endButton.GetComponentInChildren<TextMeshProUGUI>().text = "Main Menu";
        }
        endButton.onClick.AddListener(() =>
        {
            if (GameGlobals.GameConfigs.IsDemo)
                SceneManager.LoadScene("StartScene");
            else
                Application.Quit();
        });
    }
}

