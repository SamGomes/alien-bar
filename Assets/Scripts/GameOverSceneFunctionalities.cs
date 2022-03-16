using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSceneFunctionalities: MonoBehaviour
{
    public GameObject engQuestionnaire;
    public Button submitButton;
    private Slider[] engQuestions;
    public void Start()
    {
        var gm = FindObjectOfType<GameManager>();
        engQuestions = FindObjectsOfType<Slider>();
        
        submitButton.onClick.AddListener(() => {
            
            float engValue = 0.0f;
            for (int i = 0; i < engQuestions.Length; i++)
            {
                var currSlider = engQuestions[i];
                engValue += (currSlider.value/ 6.0f) / engQuestions.Length;
            }
        
            string path = "Assets/StreamingAssets/Results/results.txt";
            string json = "{ \"abilityInc\": "+float.Parse(gm.scoreValueObj.text)/ 10000.0f+
                          ",\"engagementInc\": "+engValue+",\"gradeInc\": "+0.5+"}";
            StreamWriter writer = new StreamWriter(path, true);
        
            writer.WriteLine(json);
            writer.Close();
        });
    }
    
    
    }

