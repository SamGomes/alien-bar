using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneSurvivalFunctionalities: MonoBehaviour
{
    public GameObject engQuestionnaire;
    public Button submitButton;
    private Slider[] engQuestions;
    public void Start()
    {
        
        engQuestions = FindObjectsOfType<Slider>();
        submitButton.onClick.AddListener(() => {
            
            float engValue = 0.0f;
            for (int i = 0; i < engQuestions.Length; i++)
            {
                var currSlider = engQuestions[i];
                engValue += (currSlider.value/ 6.0f) / engQuestions.Length;
            }
        
            string path = "Assets/StreamingAssets/Results/GroupResults/"+GameGlobals.PlayerId+".txt";
            string json = "{ \"engAnswers\"" + JsonConvert.SerializeObject(engQuestions)+
                          "\"abilityInc\": "+ GameGlobals.Score/ 10000.0f+
                          ",\"engagementInc\": "+engValue+"," +
                          "\"gradeInc\": "+0.5+"}";
            

            
            File.WriteAllText(path,json);
            submitButton.GetComponent<Image>().color = Color.green;
            
        });
    }
    
    
    }

