using UnityEngine;
using UnityEngine.UI;

public class NameInputManager : MonoBehaviour
{
    public InputField nameInputField;
    public Button submitButton;

    private void Start()
    {
        submitButton.onClick.AddListener(SaveName);
    }

    private void SaveName()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            // 名前をPlayFabに送信するための処理を追加
            FindObjectOfType<PlayFabLogin>().SubmitName(playerName);
        }
    }
    
}
