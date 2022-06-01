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
        resultsDisplay.text = "Score: " +
                              GameGlobals.Score;
        
        resultsDisplay.text += "\nDelivered Orders: " +
                               JsonConvert.SerializeObject(GameGlobals.NumDeliveredOrdersByLevel) +
                               "\n";
        resultsDisplay.text += "Failed Orders: " +
                               JsonConvert.SerializeObject(GameGlobals.NumFailedOrdersByLevel) +
                               "\n"; 
        
        resultsDisplay.text += "Time Spent(s): "+ Math.Round(GameGlobals.SessionTimeSpent, 3);
        

        
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
        StartCoroutine(GameGlobals.LogManager.WriteToLog("AlienBarExperiment/SURVIVAL/Results/",
            GameGlobals.ExperimentId + "_" + GameGlobals.PlayerId, logEntry));
        
        
        
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
        
            string path = "Assets/StreamingAssets/Results/GroupResults/"+GameGlobals.PlayerId+".txt";
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

