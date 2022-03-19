
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class StartSceneFunctionalities: MonoBehaviour
{
    public TMP_InputField playerIdInput;
    public TMP_Text playerId;
    public Button startGameButton;

    public void Start()
    {
        DontDestroyOnLoad(playerId.gameObject);
//        playerId.gameObject.SetActive(false);
        startGameButton.onClick.AddListener(() =>
        {
            playerId.text = playerIdInput.text;
            SceneManager.LoadScene("MainScene");
        });
    }
}
