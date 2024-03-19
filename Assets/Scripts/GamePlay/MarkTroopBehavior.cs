using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarkTroopBehavior : MonoBehaviour
{

    private GameObject troopInfoPanel;
    private TextMeshProUGUI troopInfoContent;
    private String troopInfoTemplate;
    private GameObject markedTroop;
    private MouseSelector mouseSelector;


    // Start is called before the first frame update
    void Start()
    {
        mouseSelector = Camera.main.GetComponent<MouseSelector>();

        troopInfoPanel = GameObject.FindGameObjectWithTag("TroopInfo");
        troopInfoContent = GameObject.FindGameObjectWithTag("TroopInfoContent").GetComponent<TextMeshProUGUI>();

        troopInfoTemplate = SetTroopInfoTemplate();
    }

    // Update is called once per frame
    void Update()
    {
        MarkTroop();
    }

    String SetTroopInfoTemplate() {
        return "Health: {0} \nArmor: {1} \nSpeed: {2} \nMove Range: {3} \nAttack Range: {4} \nAttack Power: {5}";
    }

    void UpdateMarkedTroop() {
        // Find marked troop via raycast
        GameObject selectedObject = mouseSelector.GetSelectedObject();

        // non-troop objects selected
        if (selectedObject == null || (!selectedObject.CompareTag("Ally") && !selectedObject.CompareTag("Enemy"))) {
            RemoveStarIfMarkedTroopNotNull();
            markedTroop = null;
        } else {
            RemoveStarIfMarkedTroopNotNull();
            markedTroop = selectedObject;
        }

    }

    void RemoveStarIfMarkedTroopNotNull() {
        if (markedTroop != null) {
            Transform healthBarCanvas = markedTroop.transform.Find("HealthBar Canvas");
            healthBarCanvas.Find("Star").gameObject.SetActive(false);
        }
    }

    void MarkTroop() {

        UpdateMarkedTroop();
        
        // Display Content
        if (markedTroop == null) {
            troopInfoPanel.SetActive(false);
        } else {
            troopInfoPanel.SetActive(true);

            Transform healthBarCanvas = markedTroop.transform.Find("HealthBar Canvas");
            healthBarCanvas.Find("Star").gameObject.SetActive(true);

            TroopBase markedTroopBase = markedTroop.GetComponent<TroopBase>();

            String content = String.Format(
                troopInfoTemplate, markedTroopBase.Health, markedTroopBase.Armor, 
                markedTroopBase.Speed, markedTroopBase.MoveRange, markedTroopBase.AttackRange, 
                markedTroopBase.AttackPower);
            
            troopInfoContent.text = content;
        }


    } 
}
