using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject equipmentButtonPrefab; // Assign in inspector
    public Transform equipmentHolder; // Assign in inspector

    public GameObject equipmentInfoScroll; // Assign in inspector
    public List<EquipmentBase> equipmentList; // Populate this list with current level equipment

    public int PriceMultiplier = 4;

    public ScoreManager scoreManager;

    void Start()
    {
        equipmentInfoScroll.SetActive(false);
        GenerateEquipmentButtons();
    }

    void GenerateEquipmentButtons()
    {
        foreach (EquipmentBase equipment in equipmentList)
        {
            // Instantiate a new button for each piece of equipment
            GameObject buttonObj = Instantiate(equipmentButtonPrefab, equipmentHolder);
            buttonObj.GetComponentInChildren<Image>().sprite = equipment.EquipmentIcon; 
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = GetEquipmentPrice(equipment) + " G";

            // Add a click listener to the button
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnEquipmentButtonClicked(equipment));
        }
    }

    void OnEquipmentButtonClicked(EquipmentBase equipment)
    {
        equipmentInfoScroll.SetActive(true);
        Debug.Log("Equipment clicked: " + equipment.name);
        ScrollRect scrollRect = equipmentInfoScroll.GetComponent<ScrollRect>();
        Transform content = scrollRect.content;
        content.Find("Header").Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;
        content.Find("Header").Find("Image").GetComponent<Image>().sprite = equipment.EquipmentIcon;
        content.Find("Description").GetComponent<TextMeshProUGUI>().text = equipment.EquipmentDescription;
        content.Find("Status").GetComponent<TextMeshProUGUI>().text = GenerateEquipmentStatus(equipment);
    }


    private int GetEquipmentPrice(EquipmentBase equipment)
    {
        var price = 0;
        price += (int)equipment.AttackPowerModifier;
        price += (int)equipment.AttackRangeModifier;
        price += (int)equipment.ArmorModifier;
        price += (int)equipment.MoveRangeModifier;
        price += (int)equipment.SpeedModifier;
        price += (int)equipment.HealthModifier / 2;
        return price * PriceMultiplier;

    }

    private String GenerateEquipmentStatus(EquipmentBase equipment)
    {
        return "Helth: " + equipment.HealthModifier + "\n" +
                "Attack Power: " + equipment.AttackPowerModifier + "\n" +
               "Attack Range: " + equipment.AttackRangeModifier + "\n" +
               "Armor: " + equipment.ArmorModifier + "\n" +
               "Move Range: " + equipment.MoveRangeModifier + "\n" +
               "Speed: " + equipment.SpeedModifier;
    }
}
