using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentManager : MonoBehaviour {


    // DeploymentManager should find out how many GameObjects with Ally tag are in the scene
    // and display that many buttons with the corresponding icons
    // When a button is clicked, the player should be able to open the equipment manager for that troop

    public List<Image> troopIcons;
    public List<Button> troopButtons;
    public List<Image> equipmentIcons;
    public List<Button> equipmentButtons;
    public Image equipmentManager;
    public Button resetEquipmentButton;
    public Button applyEquipmentButton;

    private readonly Dictionary<string, int> tagToTroopIndex = new() {
        { "TroopBtn1", 0 },
        { "TroopBtn2", 1 },
        { "TroopBtn3", 2 },
        { "TroopBtn4", 3 },
        { "TroopBtn5", 4 },
        { "TroopBtn6", 5 },
        { "TroopBtn7", 6 },
        { "TroopBtn8", 7 },
        { "TroopBtn9", 8 },
        { "TroopBtn10", 9 },
        { "TroopBtn11", 10 },
    };

    private readonly Dictionary<string, int> tagToEquipmentIndex = new() {
        { "SwordEquipment", 0 },
        { "BowEquipment", 1 },
        { "LightArmorEquipment", 2 },
        { "HeavyArmorEquipment", 3 }
    };

    private readonly Dictionary<int, EquipmentType> equipmentIndexToBodyPart = new() {
        { 0, EquipmentType.RightArm },
        { 1, EquipmentType.LeftArm },
        { 2, EquipmentType.Chest },
        { 3, EquipmentType.Chest }
    };

    // Map from indices in equipment window to EquipmentBase objects
    [Tooltip("Needs to be Populated with EquipmentBase Prefabs according to icon indices")]
    public List<EquipmentBase> equipmentObjects = new();

    // Maps Troop Index to list of equipment selected (bool), index of equipment selected matches equipment above
    private Dictionary<int, List<bool>> troopIndexToEquipmentSelected = new();
    private List<TroopBase> allies;
    private Color selectedColor = new(0, 1, 0, 0.5f);
    private int currentlySelectedTroopIndex;

    private void Awake() {
    }

    // Start is called before the first frame update
    void Start() {
        // Populate icons with buttons and add listeners
        foreach (var icon in troopIcons) {
            troopButtons.Add(icon.GetComponent<Button>());
            troopButtons[^1].onClick.AddListener(() => OnTroopButtonClicked(icon));
        }
        foreach (var icon in equipmentIcons) {
            equipmentButtons.Add(icon.GetComponent<Button>());
            equipmentButtons[^1].onClick.AddListener(() => OnEquipmentButtonClicked(icon));
        }
        resetEquipmentButton.onClick.AddListener(() => OnResetEquipmentButtonClicked());
        applyEquipmentButton.onClick.AddListener(() => OnApplyEquipmentButtonClicked());
        // Collect all TroopBase objects with Ally tag
        allies = GameObject.FindGameObjectsWithTag("Ally").Select(go => go.GetComponent<TroopBase>()).ToList();
        Debug.Log("DeploymentManager found " + allies.Count + " allies");
        // Populate equipment dictionary with no equipment
        for (int i = 0; i < allies.Count; i++) {
            troopIndexToEquipmentSelected[i] = new List<bool> { false, false, false, false };
        }
        CloseEquipmentManager();
    }

    // Update is called once per frame
    void Update() {

    }

    private void SelectTroopIcon(Image icon) {
        Image foundIcon = troopIcons.Find(i => i == icon);
        if (foundIcon == null) return;
        // If the icon is already selected, unselect it
        if (foundIcon.color == selectedColor) {
            foundIcon.color = Color.white;
        } else {
            // Change to green with transparency so image can be seen
            foundIcon.color = selectedColor;
        }
        currentlySelectedTroopIndex = troopIcons.IndexOf(foundIcon);
    }

    private void UnselectAllTroopIcons() {
        foreach (var i in troopIcons) {
            i.color = Color.white;
        }
        currentlySelectedTroopIndex = -1;
    }

    private TroopBase GetTroopFromIcon(Image troopIcon) {
        int index = tagToTroopIndex[troopIcon.tag];
        if (index >= allies.Count) return null;
        return allies[index];
    }


    public void OnTroopButtonClicked(Image icon) {
        // If the icon is already selected, unselect it and close the equipment manager
        int clickedIndex = troopIcons.IndexOf(icon);
        if (clickedIndex == -1 || clickedIndex >= allies.Count) return; // If our index can't be mapped to an ally, don't do anything
        else if (clickedIndex == currentlySelectedTroopIndex) {
            UnselectAllTroopIcons();
            CloseEquipmentManager();
        } else { // If the icon is not the currently selected one
            UnselectAllTroopIcons(); // Unselect all
            SelectTroopIcon(icon); // Select this troops
            var troop = GetTroopFromIcon(icon);
            if (troop == null) return;
            print("Selected troop: " + troop.name); // Troop should visually appear selected somehow
            OpenEquipmentManager(troop); // Open this troop's equipment Manager
        }
    }
    public void OnEquipmentButtonClicked(Image icon) {
        SelectEquipment(icon);
    }

    private void OnResetEquipmentButtonClicked() {
        // Clear dictionary for current troop
        if (troopIndexToEquipmentSelected.ContainsKey(currentlySelectedTroopIndex)) {
            troopIndexToEquipmentSelected[currentlySelectedTroopIndex] = new List<bool> { false, false, false, false };
        }
        ClearSelectedEquipment();
    }

    private void RemoveEquipment(TroopBase troop, int equipmentIndex) {
        print("Removing equipment equipped to " + troop.name + " on " + equipmentIndexToBodyPart[equipmentIndex]);
        troop.RemoveItem(equipmentIndexToBodyPart[equipmentIndex]);
    }

    private void OnApplyEquipmentButtonClicked() {
        // Obtain the current selected troop's gameobject and get the BasicSoldier reference from that GameObject
        TroopBase selectedAlly = allies[currentlySelectedTroopIndex];
        for (int i = 0; i < equipmentIcons.Count; i++) {
            if (!troopIndexToEquipmentSelected[currentlySelectedTroopIndex][i]) RemoveEquipment(selectedAlly, i);
            var equipmentGameObject = equipmentObjects[i];
            if (equipmentGameObject != null) {
                if (equipmentGameObject == null) {
                    Debug.LogError("EquipmentBase object is null, cannot apply");
                    return;
                }
                selectedAlly.EquipItem(equipmentGameObject);
                print("Applied " + equipmentGameObject.EquipmentName + " to " + selectedAlly.name);
            }
        }
    }

    private void OpenEquipmentManager(TroopBase allyTroop) {
        print("Open EquipmentManager for troop " + allyTroop.name);
        // Load the equipment for the selected troop
        ClearSelectedEquipment();
        LoadSelectedEquipment();
        equipmentManager.gameObject.SetActive(true);
        resetEquipmentButton.gameObject.SetActive(true);
        applyEquipmentButton.gameObject.SetActive(true);
    }

    private void CloseEquipmentManager() {
        equipmentManager.gameObject.SetActive(false);
        resetEquipmentButton.gameObject.SetActive(false);
        applyEquipmentButton.gameObject.SetActive(false);
    }

    private void UpdateEquipmentPolicies(int equipmentIndex) {
        var currentEquipments = troopIndexToEquipmentSelected[currentlySelectedTroopIndex];
        // If it is already selected, unselect it
        if (currentEquipments[equipmentIndex]) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][equipmentIndex] = false;
        else troopIndexToEquipmentSelected[currentlySelectedTroopIndex][equipmentIndex] = true;
        // If the opposing equipment was selected, unselect that one
        if (troopIndexToEquipmentSelected[currentlySelectedTroopIndex][equipmentIndex]) {
            if (equipmentIndex == 0) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][1] = false;
            else if (equipmentIndex == 1) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][0] = false;
            else if (equipmentIndex == 2) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][3] = false;
            else if (equipmentIndex == 3) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][2] = false;
        }
    }   

    private void SelectEquipment(Image icon) {
        // Find the Icon
        Image foundIcon = equipmentIcons.Find(i => i == icon);
        // Save the selected equipment for the current ally
        int equipmentIndex = tagToEquipmentIndex[foundIcon.tag];
        if (!troopIndexToEquipmentSelected.ContainsKey(currentlySelectedTroopIndex)) {
            troopIndexToEquipmentSelected.Add(currentlySelectedTroopIndex, new List<bool> { false, false, false, false });
        }
        UpdateEquipmentPolicies(equipmentIndex);
        // Color the icons based on selections list
        LoadSelectedEquipment();
    }

    private void ClearSelectedEquipment() {
        foreach (var i in equipmentIcons) {
            i.color = Color.white;
        }
    }

    private void LoadSelectedEquipment() {
        // Get current troop's equipment
        if (troopIndexToEquipmentSelected.ContainsKey(currentlySelectedTroopIndex)) {
            for (int i = 0; i < equipmentIcons.Count; i++) {
                // If this icon is selected, color it
                if (troopIndexToEquipmentSelected[currentlySelectedTroopIndex][i]) equipmentIcons[i].color = selectedColor;
                else equipmentIcons[i].color = Color.white;
            }
        }
    }
}
