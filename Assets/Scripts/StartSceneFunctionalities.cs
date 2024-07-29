
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
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
    public bool IsDemo;
    
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
    NONE = 0,
    DEMO = 1,
    TUTORIAL = 2,
    TRAINING = 3,
    SURVIVAL = 4
}



public static class GameGlobals
{
    public static LogManager LogManager = new FileLogManager();
    
    public static GameMode CurrGameMode = GameMode.NONE; 
        
    public static string ParticipantId;
    public static string ExperimentId;
    
    public static int AttemptId;
    public static float SessionTimeSpent;
    public static float GameModeTimeSpent;
    
    public static float Score;
    public static List<int> NumDeliveredOrdersByLevel;
    public static List<int> NumFailedOrdersByLevel;
    
    public static List<int> NumDeliveredRecipesByLevel;
    
    public static GameConfigurations GameConfigs;
    public static GameManager GameManager;
    
    
    public static bool HasPlayedDemo = true;
    public static bool HasPlayedTutorial = false;
    public static bool HasPlayedTraining = false;
    
    
    
    public static float InitialTrainingTime = -1.0f;
}



public class WaitBoardEvents : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}


public class StartSceneFunctionalities: MonoBehaviour
{
    public GameObject waitBoard;
    public GameObject demoBoard;
    
    public Slider trainingLevelInput;
    
    public TMP_InputField playerIdInput;
    public TMP_InputField experimentIdInput;
    public Button demoButton;
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

        waitBoard.AddComponent<WaitBoardEvents>();
        bool isWaiting = GameGlobals.CurrGameMode != GameMode.TRAINING ||
                         GameGlobals.HasPlayedTraining;
        isWaiting = !GameGlobals.GameConfigs.IsDemo && isWaiting;
        waitBoard.SetActive(isWaiting);
        
        demoBoard.SetActive(GameGlobals.GameConfigs.IsDemo);
        demoBoard.AddComponent<WaitBoardEvents>();
        
        tutorialButton.gameObject.AddComponent<ButtonObjectEvents>();
        trainingButton.gameObject.AddComponent<ButtonObjectEvents>();
        survivalButton.gameObject.AddComponent<ButtonObjectEvents>();
        exitButton.gameObject.AddComponent<ButtonObjectEvents>();

        if (GameGlobals.GameConfigs.IsDemo)
        {
            GameGlobals.LogManager = new DebugLogManager();
            GameGlobals.ExperimentId = "Experiment";
            GameGlobals.ParticipantId = "Participant";
        }

        if (GameGlobals.ExperimentId != "")
        {
            experimentIdInput.text = GameGlobals.ExperimentId;
        }
        if (GameGlobals.ParticipantId != "")
        {
            playerIdInput.text = GameGlobals.ParticipantId;
        }

        exitButton.interactable = false;
        exitButton.interactable = GameGlobals.GameConfigs.IsDemo || exitButton.interactable;
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        
        demoButton.interactable = !GameGlobals.HasPlayedDemo;
        demoButton.interactable = GameGlobals.GameConfigs.IsDemo || demoButton.interactable;
        demoButton.onClick.AddListener(() =>
        {
            if (experimentIdInput.text != "" && playerIdInput.text != "")
            {
                GameGlobals.ParticipantId = playerIdInput.text;
                GameGlobals.ExperimentId = experimentIdInput.text;
                GameGlobals.AttemptId = 0;

                GameGlobals.CurrGameMode = GameMode.DEMO;
                SceneManager.LoadScene("MainScene");
            }else
            {
                experimentIdInput.transform.
                    GetChild(0).gameObject.SetActive(experimentIdInput.text == "");
                playerIdInput.transform.
                    GetChild(0).gameObject.SetActive(playerIdInput.text == "");
            }
        });
        
        
        
        tutorialButton.interactable = GameGlobals.HasPlayedDemo && !GameGlobals.HasPlayedTutorial;
        tutorialButton.interactable = GameGlobals.GameConfigs.IsDemo || tutorialButton.interactable;
        tutorialButton.onClick.AddListener(() =>
        {
            if (experimentIdInput.text != "" && playerIdInput.text != "")
            {
                GameGlobals.ParticipantId = playerIdInput.text;
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
        
        
        trainingButton.interactable = GameGlobals.HasPlayedTutorial && !GameGlobals.HasPlayedTraining;
        trainingButton.interactable = GameGlobals.GameConfigs.IsDemo || trainingButton.interactable;
        trainingButton.onClick.AddListener(() =>
        {
            if (experimentIdInput.text != "" && playerIdInput.text != "")
            {
                GameGlobals.ParticipantId = playerIdInput.text;
                GameGlobals.ExperimentId = experimentIdInput.text;
                GameGlobals.AttemptId++;
                GameGlobals.GameConfigs.OrderDifficulty = (int) trainingLevelInput.value;
                
                if (GameGlobals.InitialTrainingTime < 0)
                {
                    GameGlobals.InitialTrainingTime = Time.time;
                }
                
                GameGlobals.CurrGameMode = GameMode.TRAINING;
                SceneManager.LoadScene("MainScene");
            }else
            {
                experimentIdInput.transform.
                    GetChild(0).gameObject.SetActive(experimentIdInput.text == "");
                playerIdInput.transform.
                    GetChild(0).gameObject.SetActive(playerIdInput.text == "");
            }
        });
        
        
        survivalButton.interactable = GameGlobals.HasPlayedTraining;
        survivalButton.interactable = GameGlobals.GameConfigs.IsDemo || survivalButton.interactable;
        survivalButton.onClick.AddListener(() =>
        {
            if (experimentIdInput.text != "" && playerIdInput.text != "")
            {
                GameGlobals.ParticipantId = playerIdInput.text;
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
        if (GameGlobals.GameConfigs.IsDemo || GameGlobals.InitialTrainingTime < 0)
        {
            return;
        }
        
        float playingTime = Time.time - GameGlobals.InitialTrainingTime;
        if (!GameGlobals.HasPlayedTraining && GameGlobals.CurrGameMode == GameMode.TRAINING &&
            playingTime >= GameGlobals.GameConfigs.MAXTrainingTimeMinutes * 60.0f)
        {
            GameGlobals.HasPlayedTraining = true;
            trainingButton.interactable = false;
            survivalButton.interactable = true;
            
            waitBoard.SetActive(true);
        }
    }
}
