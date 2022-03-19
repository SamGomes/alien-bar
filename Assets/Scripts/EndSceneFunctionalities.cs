using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneFunctionalities: MonoBehaviour
{
    public GameObject engQuestionnaire;
    public Button submitButton;
    private Slider[] engQuestions;
    public void Start()
    {
        TextMeshPro scoreObj = GameObject.Find("Value").GetComponent<TextMeshPro>();
        TextMeshPro playerIdObj = GameObject.Find("PlayerId").GetComponent<TextMeshPro>();
        float score = float.Parse(scoreObj.text);
        scoreObj.gameObject.SetActive(false);

        engQuestions = FindObjectsOfType<Slider>();
        
        submitButton.onClick.AddListener(() => {
            
            float engValue = 0.0f;
            for (int i = 0; i < engQuestions.Length; i++)
            {
                var currSlider = engQuestions[i];
                engValue += (currSlider.value/ 6.0f) / engQuestions.Length;
            }
        
            string path = "Assets/StreamingAssets/Results/GroupResults/"+playerIdObj.text+".txt";
            string json = "{ \"abilityInc\": "+ score/ 10000.0f+
                          ",\"engagementInc\": "+engValue+"," +
                          "\"gradeInc\": "+0.5+"}";
            

            
            File.WriteAllText(path,json);
            submitButton.GetComponent<Image>().color = Color.green;
            
        });
    }
    
    
    }

