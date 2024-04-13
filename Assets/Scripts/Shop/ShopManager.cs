using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {
    // Assign in inspector
    public GameObject equipmentButtonPrefab;
    public Transform equipmentHolder; // Equipment Scroll content.

    public GameObject equipmentInfoScroll;

    public GameObject equipmentPurchasePanel;
    public List<EquipmentBase> equipmentList; // Populate this list with current level equipment
    public List<EquipmentBase> equipmentsForThisLevel = new();

    public int PriceMultiplier = 4;

    public ScoreManager scoreManager;

    public Button shopCloseButton;

    public TextMeshProUGUI scoreText;

    private Dictionary<EquipmentBase, int> playerEquipment = new Dictionary<EquipmentBase, int>();

    public Dictionary<EquipmentBase, int> GetEquipmentCounts => playerEquipment;

    public Button closeStoryButton;

    private GameObject StoryPanel;
    private GameObject Level1StoryText;
    private GameObject Level2StoryText;
    private GameObject Level3StoryText;

    // Get the current level from SceneManagement (1, 2, 3)
    private int currentLevel => UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

    void Start() {
        if (scoreManager == null) {
            scoreManager = ScoreManager.Instance;
        }
        scoreText.text = "Score: " + scoreManager.GetScore();
        PriceMultiplier = scoreManager.GetPriceMultiplier();
        equipmentInfoScroll.SetActive(false);
        equipmentPurchasePanel.SetActive(false);
        AssignEquipmentList();
        GenerateEquipmentButtons();
        StoryPanel = GameObject.FindWithTag("StoryPanel");
        Level1StoryText = GameObject.FindWithTag("Level1StoryText");
        Level2StoryText = GameObject.FindWithTag("Level2StoryText");
        Level3StoryText = GameObject.FindWithTag("Level3StoryText");
        closeStoryButton = GameObject.FindWithTag("CloseStoryButton").GetComponent<Button>();
        closeStoryButton.onClick.AddListener(() => OnCloseStoryButtonClicked());
        ShowStory();
    }

    private void OnCloseStoryButtonClicked() {
        StoryPanel.SetActive(false);
    }

    private void AssignEquipmentList() {
        // Each equipment has a EquipmentLevel property, which is the level it is unlocked at
        // Only show equipment that is unlocked at the current level
        equipmentsForThisLevel.Clear();
        // Allow level 1 AND 2 equipment in level 3
        Debug.Log("Current Level: " + currentLevel);
        if (currentLevel == 3) equipmentsForThisLevel.AddRange(equipmentList.Where(equipment => equipment.EquipmentLevel <= currentLevel));
        else equipmentsForThisLevel.AddRange(equipmentList.Where(equipment => equipment.EquipmentLevel == currentLevel));
         Debug.Log("Equipment Count: " + equipmentsForThisLevel.Count);
    }

    private void ShowStory() {
        StoryPanel.SetActive(true);
        // Find current level from SceneManagement
        string currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentLevel == "Level1") {
            Level1StoryText.SetActive(true);
            Level2StoryText.SetActive(false);
            Level3StoryText.SetActive(false);
        } else if (currentLevel == "Level2") {
            Level1StoryText.SetActive(false);
            Level2StoryText.SetActive(true);
            Level3StoryText.SetActive(false);
        } else if (currentLevel == "Level3") {
            Level1StoryText.SetActive(false);
            Level2StoryText.SetActive(false);
            Level3StoryText.SetActive(true);
        }
    }

    void GenerateEquipmentButtons() {
        foreach (EquipmentBase equipment in equipmentsForThisLevel) {
            // Instantiate a new button for each piece of equipment
            GameObject buttonObj = Instantiate(equipmentButtonPrefab, equipmentHolder);
            buttonObj.GetComponentInChildren<Image>().sprite = equipment.EquipmentIcon;
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = GetEquipmentPrice(equipment) + " G";

            // Add a click listener to the button
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnEquipmentButtonClicked(equipment));
        }
    }

    void OnEquipmentButtonClicked(EquipmentBase equipment) {
        equipmentPurchasePanel.transform.Find("Purchase").GetComponent<Button>().onClick.RemoveAllListeners();
        equipmentPurchasePanel.transform.Find("Sell").GetComponent<Button>().onClick.RemoveAllListeners();

        equipmentInfoScroll.SetActive(true);
        Debug.Log("Equipment clicked: " + equipment.name);
        ScrollRect scrollRect = equipmentInfoScroll.GetComponent<ScrollRect>();
        Transform content = scrollRect.content;
        content.Find("Header").Find("Name").GetComponent<TextMeshProUGUI>().text = equipment.name;
        content.Find("Header").Find("Image").GetComponent<Image>().sprite = equipment.EquipmentIcon;
        content.Find("Type").GetComponent<TextMeshProUGUI>().text = equipment.EquipmentType.ToString();
        content.Find("Status").GetComponent<TextMeshProUGUI>().text = GenerateEquipmentStatus(equipment);

        equipmentPurchasePanel.SetActive(true);
        equipmentPurchasePanel.transform.Find("Purchase").GetComponent<Button>().onClick.AddListener(() => BuyEquipment(equipment));
        equipmentPurchasePanel.transform.Find("Sell").GetComponent<Button>().onClick.AddListener(() => SellEquipment(equipment));
        UpdateEquipmentPanel("Owned " + GetEquipmentQuantity(equipment));
    }


    private int GetEquipmentPrice(EquipmentBase equipment) {
        PriceMultiplier = scoreManager.GetPriceMultiplier();
        var price = 0;
        price += (int)equipment.AttackPowerModifier;
        price += (int)equipment.AttackRangeModifier / 10;
        price += (int)equipment.ArmorModifier;
        price += (int)equipment.MoveRangeModifier / 2;
        price += (int)equipment.SpeedModifier;
        price += (int)equipment.HealthModifier / 5;
        return price * PriceMultiplier;

    }

    private String GenerateEquipmentStatus(EquipmentBase equipment) {
        return "Helth: " + equipment.HealthModifier + "\n" +
                "Attack Power: " + equipment.AttackPowerModifier + "\n" +
               "Attack Range: " + equipment.AttackRangeModifier + "\n" +
               "Armor: " + equipment.ArmorModifier + "\n" +
               "Move Range: " + equipment.MoveRangeModifier + "\n" +
               "Speed: " + equipment.SpeedModifier;
    }

    public void BuyEquipment(EquipmentBase equipment) {
        //I could save this into a list to make the price faster, it is redundant here.
        int price = GetEquipmentPrice(equipment);
        if (scoreManager.GetScore() >= price) {
            scoreManager.SubtractScore(price);
            if (playerEquipment.ContainsKey(equipment)) {
                playerEquipment[equipment] += 1;
            } else {
                playerEquipment[equipment] = 1;
            }
            UpdateEquipmentPanel("Owned " + GetEquipmentQuantity(equipment));
        } else {
            UpdateEquipmentPanel("Insufficient Funds");
        }

    }

    public void SellEquipment(EquipmentBase equipment) {
        int price = GetEquipmentPrice(equipment);
        scoreManager.AddScore(price);
        if (playerEquipment.ContainsKey(equipment)) {
            playerEquipment[equipment] -= 1;
            if (playerEquipment[equipment] <= 0) {
                playerEquipment.Remove(equipment); // Remove equipment from inventory if quantity is 0
            }
        }
        UpdateEquipmentPanel("Owned " + GetEquipmentQuantity(equipment));
    }

    public int GetEquipmentQuantity(EquipmentBase equipment) {
        if (playerEquipment.ContainsKey(equipment)) {
            return playerEquipment[equipment];
        }
        return 0;
    }

    public Dictionary<EquipmentBase, int> GetInventory() {
        return playerEquipment;
    }
    //Score update is here since no matter buy or sell, the score will be updated.
    void UpdateEquipmentPanel(string text) {
        equipmentPurchasePanel.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = text;
        scoreText.text = "Score: " + scoreManager.GetScore();
    }

    public string printEquipmentList() {
        string result = "";
        foreach (var equipment in playerEquipment) {
            result += equipment.Key.name + " : " + equipment.Value + "\n";
        }
        return result;
    }
}
