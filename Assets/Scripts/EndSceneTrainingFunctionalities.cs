using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneTrainingFunctionalities: MonoBehaviour
{
    public Button restartButton;
    public void Start()
    {
        
        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("MainScene");
        });
    }
    
    
    }

