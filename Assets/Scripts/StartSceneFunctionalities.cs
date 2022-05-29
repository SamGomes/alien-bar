
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
 
    public float TrainingTimeMinutes;
    public float MAXSurvivalTimeMinutes;
    
    public int SurvivalIncreaseDifficultyDelay;
    public float SurvivalTimeChangeRate;
    
    public List<IngredientConfigurations> IngredientConfigs;
    public List<List<Recipe>> OrderRecipesByLevel;
}


public static class GameGlobals
{
    public static bool IsTraining;
    public static float PlayingTime;
    public static bool IsTutorial;
    public static string PlayerId;
    public static string GameId;
    public static float Score;
    public static GameConfigurations GameConfigs;
    public static GameManager gameManager;
    
    
    public static bool hasPlayedTutorial = false;
    public static bool hasPlayedTraining = false;
}





public class StartSceneFunctionalities: MonoBehaviour
{
    public Slider trainingLevelInput;
    
    public TMP_InputField playerIdInput;
    public TMP_InputField gameIdInput;
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

        GameGlobals.IsTutorial = false;
        

        if (GameGlobals.GameId != "")
        {
            gameIdInput.text = GameGlobals.GameId;
        }

        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        tutorialButton.interactable = !GameGlobals.hasPlayedTutorial;
        tutorialButton.onClick.AddListener(() =>
        {
            GameGlobals.PlayerId = playerIdInput.text;
            GameGlobals.IsTraining = true;
            SceneManager.LoadScene("MainScene");
        });
        
        
        trainingButton.interactable = GameGlobals.hasPlayedTutorial;
        trainingButton.onClick.AddListener(() =>
        {
            if (gameIdInput.text != "")
            {
                GameGlobals.GameConfigs.OrderDifficulty = (int) trainingLevelInput.value;
                GameGlobals.GameId = gameIdInput.text;
                GameGlobals.IsTraining = true;
                GameGlobals.IsTutorial = true;
                SceneManager.LoadScene("MainScene");
            }else
            {
                gameIdInput.transform.
                    GetChild(0).gameObject.SetActive(true);
            }
        });
        
        
        survivalButton.interactable = GameGlobals.hasPlayedTraining;
        survivalButton.onClick.AddListener(() =>
        {
//            if (playerIdInput.text != "")
//            {
                GameGlobals.PlayerId = playerIdInput.text;
                GameGlobals.IsTraining = false;
                SceneManager.LoadScene("MainScene");
//            }
//            else
//            {
//                playerIdInput.transform.
//                    GetChild(0).gameObject.SetActive(true);
//            }
        });
    }
}
