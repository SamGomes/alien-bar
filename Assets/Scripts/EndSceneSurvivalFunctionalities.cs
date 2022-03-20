using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneSurvivalFunctionalities: MonoBehaviour
{
    public GameObject thankYouObj;
    public GameObject engQuestionnaire;
    public Button submitButton;
    private Slider[] engQuestionsSliders;
    private GameObject[] engQuestions;
    
    public void Start()
    {
        
        engQuestionsSliders = engQuestionnaire.GetComponentsInChildren<Slider>();
        engQuestions = GameObject.FindGameObjectsWithTag("Question");

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
            
        });
    }
    
    
    }

