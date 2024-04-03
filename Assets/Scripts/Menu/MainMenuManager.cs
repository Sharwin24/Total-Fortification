using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for loading scenes

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject[] cameras;

    public TMP_Dropdown levelDropdown;

    public Button startButton;
    public Button quitButton;
    public Button resumeButton;

    void Start()
    {
        // Ensure the settings panel is hidden when the game starts
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Find the dropdown component and set the default value
        levelDropdown = GetComponentInChildren<TMP_Dropdown>();
        if (levelDropdown == null)
        {
            Debug.LogError("Dropdown component not found on the Canvas!");
            return;
        }
        levelDropdown.value = 0;
        levelDropdown.onValueChanged.AddListener(delegate { SetActiveCamera(levelDropdown.value); });
        SetActiveCamera(0);

        // Find the buttons and add listeners
        startButton = GameObject.Find("Start").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();
        resumeButton = GameObject.Find("Resume").GetComponent<Button>();

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ToggleSettings);
        }

    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSettings()
    {
        // Toggle the visibility of the settings panel
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // Set the active preview camera based on the index
    public void SetActiveCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(i == index);
        }
    }
}
