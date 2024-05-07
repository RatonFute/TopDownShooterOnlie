using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField = null;
    [SerializeField] private Button _continueButton = null;

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start()
    {
        SetUpInputField();
    }

    public void SetUpInputField()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        string defaultname = PlayerPrefs.GetString(PlayerPrefsNameKey);

        _nameInputField.text = defaultname;
        SetPlayerName(defaultname);
    }

    public void SetPlayerName(string name)
    {
        _continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
    {
        DisplayName = _nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    }

}
