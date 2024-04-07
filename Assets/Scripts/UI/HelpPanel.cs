using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public GameObject helpPanel;

    void Start() {
        helpPanel = GameObject.FindGameObjectWithTag("HelpPanel");
        if (helpPanel != null) helpPanel.SetActive(false);
        else Debug.LogError("HelpPanel not found");
    }

    public void OnPointerEnter(PointerEventData eventData) {
        helpPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        helpPanel.SetActive(false);
    }
}
