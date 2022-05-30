using System;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneTrainingFunctionalities: MonoBehaviour
{
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI trainingTimeDisplay;
    public Button restartButton;
    public void Start()
    {
        scoreDisplay.text = "Final Score: "+ 
                            GameGlobals.Score + " ( "+GameGlobals.NumDeliveredOrdersByLevel+" delivered orders)";
        trainingTimeDisplay.text = "Training Time (s): "+ Math.Round(GameGlobals.PlayingTime, 3);

        if (GameGlobals.IsTraining)
        {
            string path = Application.streamingAssetsPath 
                          + "/Results/AlienBarExperiment/TrainingAttempts/"
                          + GameGlobals.GameId + ".csv";

            string attemptEntry = "";
            if (!File.Exists(path))
            {
                StreamWriter writer = new StreamWriter(path, true);
                attemptEntry = "\"Lvl\";\"FinalScore\";\"NumDeliveredOrders\";\"TimeSpent\"";
                writer.WriteLine(attemptEntry);
                writer.Close();
            }
            
            StreamWriter secondWriter = new StreamWriter(path, true);
            attemptEntry = GameGlobals.GameConfigs.OrderDifficulty +
                           ";"+GameGlobals.Score +
                           ";"+GameGlobals.NumDeliveredOrdersByLevel +
                           ";"+GameGlobals.PlayingTime;
            secondWriter.WriteLine(attemptEntry);
            secondWriter.Close();
        }

        restartButton.onClick.AddListener(() => {
            SceneManager.LoadScene("StartScene");
        });
    }
}

