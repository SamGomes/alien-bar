using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneSurvivalFunctionalities: MonoBehaviour
{
    public TextMeshProUGUI resultsDisplay;
    
    public GameObject thankYouObj;
    public GameObject engQuestionnaire;
    public Button endButton;
    public Button submitButton;
    private Slider[] engQuestionsSliders;
    private GameObject[] engQuestions;
    
    public void Start()
    {
//        resultsDisplay.text = "Score: " +
//                              GameGlobals.Score;
        
        resultsDisplay.text = "\nDelivered Orders: " +
                               JsonConvert.SerializeObject(GameGlobals.NumDeliveredOrdersByLevel) +
                               "\n";
        resultsDisplay.text += "Failed Orders: " +
                               JsonConvert.SerializeObject(GameGlobals.NumFailedOrdersByLevel) +
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
        
        
        
        engQuestionsSliders = engQuestionnaire.GetComponentsInChildren<Slider>();
        engQuestions = GameObject.FindGameObjectsWithTag("Question");
        
        endButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        submitButton.onClick.AddListener(() => {
            
            float engValue = 0.0f;
            Dictionary<string,float> engQuestionnaireValues = new Dictionary<string,float>();
            for (int i = 0; i < engQuestionsSliders.Length; i++)
            {
                var currSlider = engQuestionsSliders[i];
                engValue += (currSlider.value/ 6.0f) / engQuestions.Length;
                engQuestionnaireValues.Add(engQuestions[i].GetComponent<TextMeshProUGUI>().text, currSlider.value);
            }
        
            string path = "Assets/StreamingAssets/Results/GroupResults/"+GameGlobals.ParticipantId+".txt";
            string json = "{ \"engAnswers\":" + JsonConvert.SerializeObject(engQuestionnaireValues)+
                          "\"abilityInc\": "+ GameGlobals.Score/ 10000.0f+
                          ",\"engagementInc\": "+engValue+"," +
                          "\"gradeInc\": "+0.5+"}";
            

            
            File.WriteAllText(path,json);
            submitButton.GetComponent<Image>().color = Color.green;
            engQuestionnaire.gameObject.SetActive(false);
            thankYouObj.SetActive(true);
            endButton.gameObject.SetActive(true);
        });
    }
}

