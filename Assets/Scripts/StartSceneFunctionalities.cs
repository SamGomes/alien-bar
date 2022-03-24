
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class IngredientConfigurations
{
    public List<string> IngredientPrefabs;
    public List<IngredientAttr> IngredientAttrs;
}

public class GameConfigurations
{
    public int ScoreMultiplier;
    
    public int OrderDifficulty;
    public float MINOrderTime;
    public float MAXOrderTime;
    public int MAXPendingOrders;
 
    public int SurvivalIncreaseDifficultyDelay;
    public float SurvivalDecreaseTimeRate;
    
    public List<IngredientConfigurations> IngredientConfigs;
    public List<List<Recipe>> OrderRecipesByLevel;
}


public static class GameGlobals
{
    public static bool IsTraining;
    public static string PlayerId;
    public static float Score;
    public static GameConfigurations GameConfigs;
}





public class StartSceneFunctionalities: MonoBehaviour
{
    public TMP_InputField playerIdInput;
    public Button trainingButton;
    public Button survivalButton;

    public void Start()
    {
        
        string path =  Application.streamingAssetsPath + "/configs.cfg";
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        GameGlobals.GameConfigs = 
            JsonConvert.DeserializeObject<GameConfigurations>(json);
        reader.Close();
            
        trainingButton.onClick.AddListener(() =>
        {
            GameGlobals.PlayerId = playerIdInput.text;
            GameGlobals.IsTraining = true;
            SceneManager.LoadScene("MainScene");
        });
        
        survivalButton.onClick.AddListener(() =>
        {
            if (playerIdInput.text != "")
            {
                GameGlobals.PlayerId = playerIdInput.text;
                GameGlobals.IsTraining = false;
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                playerIdInput.transform.
                    GetChild(0).gameObject.SetActive(true);
            }
        });
    }
}
