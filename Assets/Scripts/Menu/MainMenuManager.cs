using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for loading scenes

public class MainMenuManager : MonoBehaviour
{

    public GameObject[] cameras;
    public AudioClip[] cameraMusicClips;

    public TMP_Dropdown levelDropdown;



    private AudioSource audioSource;

    //public TMP_InputField inputField;

    //setting
    public Slider userScoreSlider;
    public TMP_Text userSetScoreDisplayText;
    private int userSetScore = 200;
    public Slider scoreMultiplierSlider;
    public TMP_Text scoreMultiplierDisplayText;
    private int userSetScoreMultiplier = 1;
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
    }

    private void SettingMenuInitialize()
    {
        userScoreSlider.onValueChanged.AddListener(delegate { UserSetScore(userScoreSlider.value); });

        // Initialize the display with the current slider value
        UserSetScore(userScoreSlider.value);

        scoreMultiplierSlider.onValueChanged.AddListener(delegate { UserSetScoreMultiplier(scoreMultiplierSlider.value); });
        UserSetScoreMultiplier(scoreMultiplierSlider.value);

    }
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
        ScoreManager.Instance.AddScore(userSetScore);
        ScoreManager.Instance.SetScoreMultiplier(userSetScoreMultiplier);
        SavePlayerData("Level1", userSetScore, userSetScoreMultiplier);
    }

    public void ResumeGame()
    {
        int currentScore;
        string currentLevel;
        LoadPlayerData(out currentLevel, out currentScore, out userSetScoreMultiplier);
        SceneManager.LoadScene(currentLevel);
        ScoreManager.Instance.AddScore(currentScore);
        ScoreManager.Instance.SetScoreMultiplier(userSetScoreMultiplier);

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
    public void SavePlayerData(string level, int score, int scoreMultiplier)
    {
        PlayerPrefs.SetString("CurrentLevel", level);
        PlayerPrefs.SetInt("CurrentScore", score);
        PlayerPrefs.SetInt("CurrentScoreMultiplier", scoreMultiplier);

        PlayerPrefs.Save();
    }


    public void LoadPlayerData(out string level, out int score, out int scoreMultiplier)
    {
        level = PlayerPrefs.GetString("CurrentLevel", "Level1"); 
        score = PlayerPrefs.GetInt("CurrentScore", 200); 
        scoreMultiplier = PlayerPrefs.GetInt("CurrentScoreMultiplier", 1); 
    }


    public void UserSetScore(float value)
    {
        // Update the text to show the current value of the slider
        userSetScore = (int)value;
        userSetScoreDisplayText.text = "Initial Score:" + userSetScore.ToString();

        Debug.Log("User set score: " + userSetScore);
    }

    public void UserSetScoreMultiplier(float value)
    {
        // Update the text to show the current value of the slider
        userSetScoreMultiplier = (int)value;
        scoreMultiplierDisplayText.text = "Initial Score:" + userSetScoreMultiplier.ToString();

        Debug.Log("User set score: " + userSetScoreMultiplier);
    }

    public void fakeSave(){
        SavePlayerData("Level2", 2300,3);
    }
}
