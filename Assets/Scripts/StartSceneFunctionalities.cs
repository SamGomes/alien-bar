
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ButtonObjectEvents : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<AudioSource>().Play();
    }
}

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
 
    public float TutorialTimeMinutes;
    public float MAXTrainingTimeMinutes;
    public float MAXSurvivalTimeMinutes;
    
    public int SurvivalIncreaseDifficultyDelay;
    public float SurvivalTimeChangeRate;
    
    public List<IngredientConfigurations> IngredientConfigs;
    public List<List<Recipe>> OrderRecipesByLevel;
}

public enum GameMode{
    TUTORIAL = 0,
    TRAINING = 1,
    SURVIVAL = 2
}


public static class GameGlobals
{
    public static LogManager LogManager = new FileLogManager();
    
    public static GameMode CurrGameMode; //0 - tutorial; 1 - training; 2 - survival
    
    public static string PlayerId;
    public static string ExperimentId;
    
    public static int AttemptId;
    public static float PlayingTime;
    public static float Score;
    public static List<int> NumDeliveredOrdersByLevel;
    public static List<int> NumFailedOrdersByLevel;
    
    public static GameConfigurations GameConfigs;
    public static GameManager gameManager;
    
    
    public static bool hasPlayedTutorial = false;
    public static bool hasPlayedTraining = false;
    
    
    
    public static float initialTrainingTime = -1.0f;
}





public class StartSceneFunctionalities: MonoBehaviour
{
    public Slider trainingLevelInput;
    
    public TMP_InputField playerIdInput;
    public TMP_InputField experimentIdInput;
    public Button tutorialButton;
    public Button trainingButton;
    public Button survivalButton;
    
    public Button exitButton;

    public void Start()
    {

        string path = Application.streamingAssetsPath + "/configs.cfg";
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        GameGlobals.GameConfigs =
            JsonConvert.DeserializeObject<GameConfigurations>(json);
        reader.Close();

        tutorialButton.gameObject.AddComponent<ButtonObjectEvents>();
        trainingButton.gameObject.AddComponent<ButtonObjectEvents>();
        survivalButton.gameObject.AddComponent<ButtonObjectEvents>();
        exitButton.gameObject.AddComponent<ButtonObjectEvents>();

        GameGlobals.CurrGameMode = GameMode.TUTORIAL;
        

        if (GameGlobals.ExperimentId != "")
        {
            experimentIdInput.text = GameGlobals.ExperimentId;
        }
        if (GameGlobals.PlayerId != "")
        {
            playerIdInput.text = GameGlobals.PlayerId;
        }

        exitButton.interactable = false;
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        tutorialButton.interactable = !GameGlobals.hasPlayedTutorial;
        tutorialButton.onClick.AddListener(() =>
        {
            if (experimentIdInput.text != "" && playerIdInput.text != "")
            {
                GameGlobals.PlayerId = playerIdInput.text;
                GameGlobals.ExperimentId = experimentIdInput.text;
                GameGlobals.AttemptId = 0;

                GameGlobals.CurrGameMode = GameMode.TUTORIAL;
                SceneManager.LoadScene("MainScene");
            }else
            {
                experimentIdInput.transform.
                    GetChild(0).gameObject.SetActive(experimentIdInput.text == "");
                playerIdInput.transform.
                    GetChild(0).gameObject.SetActive(playerIdInput.text == "");
            }
        });
        
        
        trainingButton.interactable = GameGlobals.hasPlayedTutorial && !GameGlobals.hasPlayedTraining;
        trainingButton.onClick.AddListener(() =>
        {
            if (experimentIdInput.text != "" && playerIdInput.text != "")
            {
                GameGlobals.AttemptId++;
                
                GameGlobals.GameConfigs.OrderDifficulty = (int) trainingLevelInput.value;
                GameGlobals.CurrGameMode = GameMode.TRAINING;
                SceneManager.LoadScene("MainScene");

                if (GameGlobals.initialTrainingTime < 0)
                {
                    GameGlobals.initialTrainingTime = Time.time;
                }
            }else
            {
                experimentIdInput.transform.
                    GetChild(0).gameObject.SetActive(experimentIdInput.text == "");
                playerIdInput.transform.
                    GetChild(0).gameObject.SetActive(playerIdInput.text == "");
            }
        });
        
        
        survivalButton.interactable = GameGlobals.hasPlayedTraining;
        survivalButton.onClick.AddListener(() =>
        {
            if (experimentIdInput.text != "" && playerIdInput.text != "")
            {
                GameGlobals.PlayerId = playerIdInput.text;
                GameGlobals.ExperimentId = experimentIdInput.text;
                GameGlobals.AttemptId = 0;
                
                GameGlobals.CurrGameMode = GameMode.SURVIVAL;
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                experimentIdInput.transform.
                    GetChild(0).gameObject.SetActive(experimentIdInput.text == "");
                playerIdInput.transform.
                    GetChild(0).gameObject.SetActive(playerIdInput.text == "");
            }
        });
        
        
    }
    public void Update()
    {
        float _playingTime = Time.time - GameGlobals.initialTrainingTime;
        if (GameGlobals.CurrGameMode == GameMode.TRAINING &&
            _playingTime >= GameGlobals.GameConfigs.MAXTrainingTimeMinutes * 60.0f)
        {
            GameGlobals.hasPlayedTraining = true;
            trainingButton.interactable = GameGlobals.hasPlayedTutorial && !GameGlobals.hasPlayedTraining;
            survivalButton.interactable = GameGlobals.hasPlayedTraining;
        }
    }
}
