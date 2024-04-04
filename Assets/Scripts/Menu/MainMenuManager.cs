using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI; // Required for loading scenes

public class MainMenuManager : MonoBehaviour
{

    public GameObject[] cameras;
    public AudioClip[] cameraMusicClips;

    public TMP_Dropdown levelDropdown;

    public TMP_Text playerProgressText;



    private AudioSource audioSource;

    //public TMP_InputField inputField;

    //setting
    public Slider userScoreSlider;
    public TMP_Text userSetScoreDisplayText;
    private int userSetScore = 200;
    public Slider scoreMultiplierSlider;
    public TMP_Text scoreMultiplierDisplayText;
    private int userSetScoreMultiplier = 1;

    public Slider equipmentPriceMultiplierSlider;
    public TMP_Text equipmentPriceMultiplierText;
    private int userSetEquipmentPriceMultiplier = 1;

    public Slider volumeSlider;
    public TMP_Text userSetVolumeText;
    private float volume = 1;
    void Start()
    {

        MainMenuInitialize();
        SettingMenuInitialize();
    }

    private void MainMenuInitialize()
    {


        // Find the dropdown component and set the default value
        levelDropdown = GetComponentInChildren<TMP_Dropdown>();
        if (levelDropdown == null)
        {
            Debug.LogError("Dropdown component not found on the Canvas!");
            return;
        }
        levelDropdown.value = 0;
        levelDropdown.onValueChanged.AddListener(delegate { SetActiveCamera(levelDropdown.value); });
        //For background music
        audioSource = FindObjectOfType<AudioSource>();
        SetActiveCamera(0);
        // Load the player data
        LoadPlayerData(out string currentLevel, out int currentScore, out int currentScoreMultiplier, out int currentEquipmentPriceMultiplier);
        playerProgressText.text = "Progress History: " + currentLevel + " Score: " + currentScore;
    }

    private void SettingMenuInitialize()
    {
        userScoreSlider.onValueChanged.AddListener(delegate { UserSetScore(userScoreSlider.value); });

        // Initialize the display with the current slider value
        UserSetScore(userScoreSlider.value);

        scoreMultiplierSlider.onValueChanged.AddListener(delegate { UserSetScoreMultiplier(scoreMultiplierSlider.value); });
        UserSetScoreMultiplier(scoreMultiplierSlider.value);

        equipmentPriceMultiplierSlider.onValueChanged.AddListener(delegate { UserSetEquipmentPriceMultiplier(equipmentPriceMultiplierSlider.value); });
        UserSetEquipmentPriceMultiplier(equipmentPriceMultiplierSlider.value);

        volumeSlider.onValueChanged.AddListener(delegate { UserSetVolume(volumeSlider.value); });
        UserSetVolume(volumeSlider.value);

    }
    public void StartGame()
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        ScoreManager.Instance.AddScore(userSetScore);
        ScoreManager.Instance.SetScoreMultiplier(userSetScoreMultiplier);
        ScoreManager.Instance.SetPriceMultiplier(userSetEquipmentPriceMultiplier);
        Debug.Log("Score: " + userSetScore + " Score Multiplier: " + userSetScoreMultiplier + " Equipment Price Multiplier: " + userSetEquipmentPriceMultiplier);
        SavePlayerData("Level1", userSetScore, userSetScoreMultiplier, userSetEquipmentPriceMultiplier);
        SceneManager.LoadScene("Level1");
    }

    public void ResumeGame()
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        LoadPlayerData(out string currentLevel, out int currentScore, out userSetScoreMultiplier, out userSetEquipmentPriceMultiplier);      
        ScoreManager.Instance.AddScore(currentScore);
        ScoreManager.Instance.SetScoreMultiplier(userSetScoreMultiplier);
        ScoreManager.Instance.SetPriceMultiplier(userSetEquipmentPriceMultiplier);

        SceneManager.LoadScene(currentLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Set the active preview camera based on the index
    public void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(i == index);

        }
        audioSource.clip = cameraMusicClips[index];
        audioSource.Play();
    }

    //Save whenever the player changes the level
    public void SavePlayerData(string level, int score, int scoreMultiplier, int equipmentPriceMultiplier)
    {
        PlayerPrefs.SetString("CurrentLevel", level);
        PlayerPrefs.SetInt("CurrentScore", score);
        PlayerPrefs.SetInt("CurrentScoreMultiplier", scoreMultiplier);
        PlayerPrefs.SetInt("CurrentEquipmentPriceMultiplier", equipmentPriceMultiplier);
        PlayerPrefs.Save();
    }


    public void LoadPlayerData(out string level, out int score, out int scoreMultiplier, out int equipmentPriceMultiplier)
    {
        level = PlayerPrefs.GetString("CurrentLevel", "Level1");
        score = PlayerPrefs.GetInt("CurrentScore", 200);
        scoreMultiplier = PlayerPrefs.GetInt("CurrentScoreMultiplier", 1);
        equipmentPriceMultiplier = PlayerPrefs.GetInt("CurrentEquipmentPriceMultiplier", 1);
    }


    public void UserSetScore(float value)
    {
        // Update the text to show the current value of the slider
        userSetScore = (int)value;
        userSetScoreDisplayText.text = "Initial Score:" + userSetScore.ToString();
    }

    public void UserSetScoreMultiplier(float value)
    {
        // Update the text to show the current value of the slider
        userSetScoreMultiplier = (int)value;
        scoreMultiplierDisplayText.text = "Score Multiplier:" + userSetScoreMultiplier.ToString();
    }

    public void UserSetEquipmentPriceMultiplier(float value)
    {
        // Update the text to show the current value of the slider
        userSetEquipmentPriceMultiplier = (int)value;
        equipmentPriceMultiplierText.text = "Price Multiplier: " + userSetEquipmentPriceMultiplier.ToString();
    }

    public void UserSetVolume(float value)
    {
        // Update the text to show the current value of the slider
        volume = value;
        userSetVolumeText.text = "Volume: " + volume.ToString();
        audioSource.volume = volume;
    }

    public void fakeSave()
    {
        SavePlayerData("Level2", 2300, 3, 1);
    }
}
