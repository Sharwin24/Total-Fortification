using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour {
    public GameObject helpPanel;
    private Button helpButton;
    private Color defaultColor;

    void Start() {
        // Button is child of this component
        helpButton = GetComponentInChildren<Button>();
        if (helpPanel != null) helpPanel.SetActive(false);
        else Debug.LogError("HelpPanel not found");
        if (helpButton != null) helpButton.onClick.AddListener(ToggleHelpPanel);
        else Debug.LogError("HelpButton not found");
        this.defaultColor = helpButton.image.color;
    }

    void ToggleHelpPanel() {
        if (helpPanel.activeSelf) helpPanel.SetActive(false);
        else helpPanel.SetActive(true);
        ColorIcon();
    }

    void ColorIcon() {
        // If the help panel is active, change the color of the help button to a slight green, otherwise default color
        if (helpPanel.activeSelf) helpButton.image.color = new Color(0.8f, 1f, 0.8f, 1f);
        else helpButton.image.color = this.defaultColor;
    }
}
