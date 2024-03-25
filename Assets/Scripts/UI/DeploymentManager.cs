using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using static UnityEditor.ShaderData;

public class DeploymentManager : MonoBehaviour {

    /*
    Deployment UI:
    - Deploy Troop on different places by Player(drag or drop, or clicking)
    - Player only have a certain number of equipments available, for example, only 5 heavy chest armor.
    - Troop Highlight when selected(a gem on their head). For both phases.
    - Equipments UI should be in the form of Head, Chest, Left Arm, Right Arm, and Legs.Then the player can choose equipment for these parts.With Troop Status like this.
    - When the user clicks on one of these bodyparts. they will only see weapons/armor available at those parts. 
    Implementation detail:
    - Combine the EquipmentIcon with the Equipment Object into one list. 
    */

    public List<Button> troopButtons;
    public List<Button> equipmentButtons;
    public Image equipmentWindowManager;
    public Image equipmentItemsWindow;
    public Button resetEquipmentButton;
    public Button applyEquipmentButton;
    public EquipmentType equipmentTypeSelected = EquipmentType.None;

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

    private readonly Dictionary<string, EquipmentType> tagToEquipmentType = new() {
        { "HeadEquipmentBtn", EquipmentType.Head },
        { "ChestEquipmentBtn", EquipmentType.Chest },
        { "LeftArmEquipmentBtn", EquipmentType.LeftArm },
        { "RightArmEquipmentBtn", EquipmentType.RightArm },
        { "LegsEquipmentBtn", EquipmentType.Legs }
    };

    // Map from indices in equipment window to EquipmentBase objects
    [Tooltip("Needs to be Populated with EquipmentBase Prefabs and count")]
    public Dictionary<EquipmentBase, int> equipmentInventory = new();

    // Maps Troop Index to list of equipment selected (bool), index of equipment selected matches equipment above
    private List<TroopBase> allies;
    private Color selectedColor = new(0, 1, 0, 0.5f);
    private int currentlySelectedTroopIndex;

    private void Awake() {
    }

    void SetupButtons() {
        // The troop buttons are tagged TroopBtn1, TroopBtn2, etc.
        // so we can find them by tag and add them to the list
        for (int i = 1; i <= tagToTroopIndex.Count; i++) {
            string tag = "TroopBtn" + i;
            GameObject go = GameObject.FindGameObjectWithTag(tag);
            if (go != null) {
                Button button = go.GetComponent<Button>();
                if (button != null) {
                    troopButtons.Add(button);
                    troopButtons[^1].onClick.AddListener(() => OnTroopButtonClicked(button));
                }
            }
        }
        // Equipment Type Buttons are tagged HeadEquipmentBtn, ChestEquipmentBtn, etc.
        for (int i = 0; i < tagToEquipmentType.Count; i++) {
            string tag = tagToEquipmentType.ElementAt(i).Key;
            GameObject go = GameObject.FindGameObjectWithTag(tag);
            if (go != null) {
                Button button = go.GetComponent<Button>();
                if (button != null) {
                    equipmentButtons.Add(button);
                    equipmentButtons[^1].onClick.AddListener(() => OnEquipmentButtonClicked(button));
                }
            }
        }
        // Reset and Apply buttons
        resetEquipmentButton.onClick.AddListener(() => OnResetEquipmentButtonClicked());
        //applyEquipmentButton.onClick.AddListener(() => OnApplyEquipmentButtonClicked());
    }

    // Start is called before the first frame update
    void Start() {
        // Populate icons with buttons and add listeners
        SetupButtons();
        // Collect all TroopBase objects with Ally tag
        allies = GameObject.FindGameObjectsWithTag("Ally").Select(go => go.GetComponent<TroopBase>()).ToList();
        Debug.Log("DeploymentManager found " + allies.Count + " allies");
        CloseEquipmentManager();
    }

    // Update is called once per frame
    void Update() {

    }

    private void HighlightSelectedTroop(bool enable) {
        if (currentlySelectedTroopIndex == -1) return;
        var selectedTroop = allies[currentlySelectedTroopIndex];
        // Get the SelectedIndicatorGem object
        // Find child object of selectedTroop with tag "SelectionIndicatorGem"
        GameObject gem = selectedTroop.transform.Find("SelectionIndicatorGem")?.gameObject;
        //GameObject gem = selectedTroop.GetComponentInChildren<GemIndicator>()?.gameObject;
        if (gem == null) return;
        // Enable the GemIndicator
        gem.SetActive(enable);
    }

    private void SelectTroopIcon(Button btn) {
        Image image = btn.GetComponent<Image>();
        if (image == null) return;
        currentlySelectedTroopIndex = tagToTroopIndex[btn.tag];
        // If the icon is already selected, unselect it
        if (image.color == selectedColor) {
            image.color = Color.white;
            HighlightSelectedTroop(false);
        } else {
            // Change to green with transparency so image can be seen
            image.color = selectedColor;
            HighlightSelectedTroop(true);
        }
    }

    private void UnselectAllTroopIcons() {
        foreach (var btn in troopButtons) {
            Image i = btn.GetComponent<Image>();
            i.color = Color.white;
        }
        HighlightSelectedTroop(false);
        currentlySelectedTroopIndex = -1;
    }

    private TroopBase GetTroopFromIcon(Button btn) {
        int index = tagToTroopIndex[btn.tag];
        if (index >= allies.Count) return null;
        return allies[index];
    }


    public void OnTroopButtonClicked(Button btn) {
        // If the icon is already selected, unselect it and close the equipment manager
        int clickedIndex = tagToTroopIndex[btn.tag];
        if (clickedIndex == -1 || clickedIndex >= allies.Count) return; // If our index can't be mapped to an ally, don't do anything
        else if (clickedIndex == currentlySelectedTroopIndex) {
            UnselectAllTroopIcons();
            CloseEquipmentManager();
        } else { // If the icon is not the currently selected one
            UnselectAllTroopIcons(); // Unselect all
            SelectTroopIcon(btn); // Select this troops
            var troop = GetTroopFromIcon(btn);
            if (troop == null) return;
            print("Selected troop: " + troop.name); // Troop should visually appear selected somehow
            OpenEquipmentManager(troop); // Open this troop's equipment Manager
        }
    }
    public void OnEquipmentButtonClicked(Button btn) {
        // Find the Icon
        Image image = btn.GetComponent<Image>();
        if (image.color == selectedColor) {
            image.color = Color.white;
            equipmentTypeSelected = EquipmentType.None;
        } else {
            // Change to green with transparency so image can be seen
            image.color = selectedColor;
            equipmentTypeSelected = tagToEquipmentType[btn.tag];
            Debug.Log("Equipment Type: " + equipmentTypeSelected + " selected");
        }
        //UpdateEquipmentPolicies(equipmentIndex);
        // Color the icons based on selections list
        //LoadSelectedEquipment();
    }

    private void OnResetEquipmentButtonClicked() {
        ClearSelectedEquipment();
    }

    private void ClearSelectedEquipment() {
        foreach (var btn in equipmentButtons) {
            Image i = btn.GetComponent<Image>();
            i.color = Color.white;
        }
    }

    public EquipmentType GetEquipmentTypeSelected() {
        return equipmentTypeSelected;
    }

    //private void RemoveEquipment(TroopBase troop, int equipmentIndex) {
    //    print("Removing equipment equipped to " + troop.name + " on " + equipmentIndexToBodyPart[equipmentIndex]);
    //    troop.RemoveItem(equipmentIndexToBodyPart[equipmentIndex]);
    //}

    //private void OnApplyEquipmentButtonClicked() {
    //    // Obtain the current selected troop's gameobject and get the BasicSoldier reference from that GameObject
    //    TroopBase selectedAlly = allies[currentlySelectedTroopIndex];
    //    for (int i = 0; i < equipmentIcons.Count; i++) {
    //        if (!troopIndexToEquipmentSelected[currentlySelectedTroopIndex][i]) continue;//RemoveEquipment(selectedAlly, i);
    //        var equipmentGameObject = equipmentObjects[i];
    //        if (equipmentGameObject != null) {
    //            if (equipmentGameObject == null) {
    //                Debug.LogError("EquipmentBase object is null, cannot apply");
    //                return;
    //            }
    //            selectedAlly.EquipItem(equipmentGameObject);
    //            print("Applied " + equipmentGameObject.EquipmentName + " to " + selectedAlly.name);
    //        }
    //    }
    //}

    private void OpenEquipmentManager(TroopBase allyTroop) {
        print("Open EquipmentManager for troop " + allyTroop.name);
        // Load the equipment for the selected troop
        ClearSelectedEquipment();
        equipmentWindowManager.gameObject.SetActive(true);
        equipmentItemsWindow.gameObject.SetActive(true);
        resetEquipmentButton.gameObject.SetActive(true);
        applyEquipmentButton.gameObject.SetActive(true);
    }

    private void CloseEquipmentManager() {
        print("Close EquipmentManager");
        equipmentWindowManager.gameObject.SetActive(false);
        equipmentItemsWindow.gameObject.SetActive(false);
        resetEquipmentButton.gameObject.SetActive(false);
        applyEquipmentButton.gameObject.SetActive(false);
    }

    private void LoadEquipmentManager(TroopBase troop) {

    }

    //private void UpdateEquipmentPolicies(int equipmentIndex) {
    //    var currentEquipments = troopIndexToEquipmentSelected[currentlySelectedTroopIndex];
    //    // If it is already selected, unselect it
    //    if (currentEquipments[equipmentIndex]) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][equipmentIndex] = false;
    //    else troopIndexToEquipmentSelected[currentlySelectedTroopIndex][equipmentIndex] = true;
    //    // If the opposing equipment was selected, unselect that one
    //    if (troopIndexToEquipmentSelected[currentlySelectedTroopIndex][equipmentIndex]) {
    //        if (equipmentIndex == 0) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][1] = false;
    //        else if (equipmentIndex == 1) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][0] = false;
    //        else if (equipmentIndex == 2) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][3] = false;
    //        else if (equipmentIndex == 3) troopIndexToEquipmentSelected[currentlySelectedTroopIndex][2] = false;
    //    }
    //}
}
