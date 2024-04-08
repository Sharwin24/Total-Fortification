using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DeploymentManager : MonoBehaviour {

    public GameObject troopInfoPanel;
    public TextMeshProUGUI troopInfoContent;
    public List<Button> troopButtons;
    public List<Button> equipmentSlotButtons;
    public Image equipmentWindowManager;
    public TextMeshProUGUI equipmentTypeText;
    public Image equipmentItemsWindow;
    public Button resetEquipmentButton;
    public EquipmentType EquipmentTypeSelected = EquipmentType.None;
    public TroopBase GetSelectedTroop => currentlySelectedTroopIndex != -1 && allies.Count > 0 ? allies[currentlySelectedTroopIndex] : null;
    public Dictionary<EquipmentBase, int> EquipmentInventory = new();
    public Button shopCloseButton;


    // Empty Slot Sprites
    public Sprite headEmptySlotSprite;
    public Sprite chestEmptySlotSprite;
    public Sprite rightArmEmptySlotSprite;
    public Sprite leftArmEmptySlotSprite;
    public Sprite legsEmptySlotSprite;

    private GameObject troopToDeployPrefab; // Disabled

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
        { "TroopBtn12", 11 },
        { "TroopBtn13", 12 },
        { "TroopBtn14", 13 },
    };

    private readonly Dictionary<string, EquipmentType> tagToEquipmentType = new() {
        { "HeadEquipmentBtn", EquipmentType.Head },
        { "ChestEquipmentBtn", EquipmentType.Chest },
        { "LeftArmEquipmentBtn", EquipmentType.LeftArm },
        { "RightArmEquipmentBtn", EquipmentType.RightArm },
        { "LegsEquipmentBtn", EquipmentType.Legs }
    };

    private readonly Dictionary<EquipmentType, Sprite> equipmentTypeToSprite = new() {
        { EquipmentType.Head, null },
        { EquipmentType.Chest, null },
        { EquipmentType.LeftArm, null },
        { EquipmentType.RightArm, null },
        { EquipmentType.Legs, null}
    };

    private String troopInfoTemplate;
    private List<EquipmentButtonBehavior> equipmentBtnBehaviors = new();
    private List<TroopBase> allies = new();
    private Dictionary<TroopBase, List<IEquipment>> troopEquipmentMemory = new();
    private Color selectedIconColor = new(0, 1, 0, 0.5f);
    private int currentlySelectedTroopIndex;
    private MouseSelector mouseSelector;
    private ShopManager shopManager;


    private void SetupButtons() {
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
                    equipmentSlotButtons.Add(button);
                    equipmentSlotButtons[^1].onClick.AddListener(() => OnEquipmentSlotButtonClicked(button));
                }
            }
        }
        // Reset button and ShopCloseButton
        resetEquipmentButton.onClick.AddListener(() => OnResetEquipmentButtonClicked());
        shopCloseButton.onClick.AddListener(() => OnShopCloseButtonClicked());
        // Equipment Buttons
        this.equipmentBtnBehaviors = GameObject.FindObjectsByType<EquipmentButtonBehavior>(FindObjectsSortMode.None).ToList();
        Debug.Log("Deploymentmanager Found " + equipmentBtnBehaviors.Count + " equipment buttons");
    }

    // Start is called before the first frame update
    void Start() {
        troopInfoTemplate = "Health: {0} \nArmor: {1} \nSpeed: {2} \nMove Range: {3} \nAttack Range: {4} \nAttack Power: {5}";
        if (troopInfoPanel != null) troopInfoPanel.SetActive(false);
        else Debug.LogError("Troop Info Panel not found");
        // Collect all TroopBase objects with Ally tag
        allies = GameObject.FindGameObjectsWithTag("Ally").Select(go => go.GetComponent<TroopBase>()).ToList();
        Debug.Log("DeploymentManager found " + allies.Count + " allies");
        CloseEquipmentManager();
        // Setup Equipment Types
        equipmentTypeToSprite[EquipmentType.Head] = headEmptySlotSprite;
        equipmentTypeToSprite[EquipmentType.Chest] = chestEmptySlotSprite;
        equipmentTypeToSprite[EquipmentType.LeftArm] = leftArmEmptySlotSprite;
        equipmentTypeToSprite[EquipmentType.RightArm] = rightArmEmptySlotSprite;
        equipmentTypeToSprite[EquipmentType.Legs] = legsEmptySlotSprite;
        // Clear all equipment slots
        foreach (var type in Enum.GetValues(typeof(EquipmentType))) {
            ClearEquippedSlot((EquipmentType)type);
        }
        // Find Mouse Selector
        if (!Camera.main.TryGetComponent<MouseSelector>(out mouseSelector)) Debug.LogError("MouseSelector not found");
        // Find ShopManager and shopCloseButton
        shopManager = GameObject.FindGameObjectWithTag("ShopUI").GetComponent<ShopManager>();
        shopCloseButton = GameObject.FindGameObjectWithTag("ShopCloseButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update() {
        // If right click is pressed, deploy the selected troop
        // if (Input.GetMouseButtonDown(1)) DeployTroopOnMap();
    }

    private void OnEnable() {
        // Populate icons with buttons and add listeners
        SetupButtons();
    }

    private void OnShopCloseButtonClicked() {
        SetEquipmentsFromShop(shopManager.GetEquipmentCounts);
    }

    private void DeployTroopOnMap() {
        // Deploys the selected troop on the map at the selected position
        Vector3 pos = mouseSelector.GetSelectedPosition();
        if (this.GetSelectedTroop == null || mouseSelector == null || troopToDeployPrefab == null || pos == null) return;
        // Instantiate the selected troop at the selected position
        GameObject go = Instantiate(troopToDeployPrefab, pos, Quaternion.identity);
        TroopBase troop = go.GetComponent<TroopBase>();
        // TODO: Dynamically create a troopbutton for this troop
        // We might need to rethink how the troopButtons are created so we can dynamically add and remove them from the window
    }

    public void SetTroopInfo(TroopBase troop) {
        // non-troop objects selected
        if (troop == null) {
            troopInfoPanel.SetActive(false);
            return;
        } else if (troop.CompareTag("Ally") || troop.CompareTag("Enemy")) {
            troopInfoPanel.SetActive(true);
            String content = String.Format(
                troopInfoTemplate, troop.Health, troop.Armor,
                troop.Speed, troop.MoveRange, troop.AttackRange,
                troop.AttackPower);
            troopInfoContent.text = content;
        }
    }

    public void AssignEquippedSlot(EquipmentType type, IEquipment item) {
        if (type == EquipmentType.None || item == null) return;
        // If type is TwoHanded, assign to both LeftArm and RightArm
        if (type == EquipmentType.TwoHanded) {
            Image leftImage = equipmentSlotButtons.Find((btn) => tagToEquipmentType[btn.tag] == EquipmentType.LeftArm).GetComponent<Image>();
            Image rightImage = equipmentSlotButtons.Find((btn) => tagToEquipmentType[btn.tag] == EquipmentType.RightArm).GetComponent<Image>();
            leftImage.sprite = item.EquipmentIcon;
            rightImage.sprite = item.EquipmentIcon;
        } else {
            Image i = equipmentSlotButtons.Find((btn) => tagToEquipmentType[btn.tag] == type).GetComponent<Image>();
            // Set image to item
            i.sprite = item.EquipmentIcon;
        }
    }

    public void ClearEquippedSlot(EquipmentType type) {
        if (type == EquipmentType.None || !this.equipmentWindowManager.IsActive()) return;
        if (type == EquipmentType.TwoHanded) {
            // Find left and right arm buttons
            Image leftImage = equipmentSlotButtons.Find((btn) => tagToEquipmentType[btn.tag] == EquipmentType.LeftArm).GetComponent<Image>();
            Image rightImage = equipmentSlotButtons.Find((btn) => tagToEquipmentType[btn.tag] == EquipmentType.RightArm).GetComponent<Image>();
            leftImage.sprite = equipmentTypeToSprite[EquipmentType.LeftArm];
            rightImage.sprite = equipmentTypeToSprite[EquipmentType.RightArm];
        } else {
            Image i = equipmentSlotButtons.Find((btn) => tagToEquipmentType[btn.tag] == type).GetComponent<Image>();
            // Set image to item
            i.sprite = equipmentTypeToSprite[type];
        }
    }

    private void HighlightSelectedTroop(bool enable) {
        if (currentlySelectedTroopIndex == -1) return;
        var selectedTroop = allies[currentlySelectedTroopIndex];
        // Get the SelectedIndicatorGem object
        // Find child object of selectedTroop with tag "SelectionIndicatorGem"
        GameObject gem = selectedTroop.transform.Find("SelectionIndicatorGem").gameObject;
        if (gem == null) return;
        // Enable the GemIndicator
        gem.SetActive(enable);
    }

    private void SelectTroopIcon(Button btn) {
        Image image = btn.GetComponent<Image>();
        if (image == null) return;
        currentlySelectedTroopIndex = tagToTroopIndex[btn.tag];
        // If the icon is already selected, unselect it
        if (image.color == selectedIconColor) {
            image.color = Color.white;
            HighlightSelectedTroop(false);
        } else {
            // Change to green with transparency so image can be seen
            image.color = selectedIconColor;
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
            TroopBase troop = GetTroopFromIcon(btn);
            if (troop == null) return;
            Debug.Log("Troop Button Clicked: " + troop.name); // Troop should visually appear selected somehow
            OpenEquipmentManager(troop); // Open this troop's equipment Manager
        }
    }
    public void OnEquipmentSlotButtonClicked(Button btn) {
        // Find the Icon
        Image image = btn.GetComponent<Image>();
        if (image.color == selectedIconColor) {
            image.color = Color.white;
            EquipmentTypeSelected = EquipmentType.None;
        } else {
            // Check if any other slots are already selected and if so, clear them
            foreach (var b in equipmentSlotButtons) {
                Image i = b.GetComponent<Image>();
                if (i.color == selectedIconColor) i.color = Color.white;
            }
            // Change to green with transparency so image can be seen
            image.color = selectedIconColor;
            EquipmentTypeSelected = tagToEquipmentType[btn.tag];
            Debug.Log("Equipment Type: " + EquipmentTypeSelected + " selected");
        }
        SetTypeText();
        foreach (var ebb in this.equipmentBtnBehaviors) {
            ebb.EnableByType(EquipmentTypeSelected);
        }
        UpdateEquipmentSelected();
    }

    private void OnResetEquipmentButtonClicked() {
        ClearSelectedEquipment();
    }

    public EquipmentType GetEquipmentTypeSelected() {
        return EquipmentTypeSelected;
    }

    private void SetTypeText() {
        if (this.EquipmentTypeSelected == EquipmentType.None) {
            equipmentTypeText.text = "Select Equipment Type";
        } else {
            equipmentTypeText.text = this.EquipmentTypeSelected.ToString();
        }
    }

    private void ClearSelectedEquipment() {
        EquipmentTypeSelected = EquipmentType.None;
        SetTypeText();
        foreach (var btn in equipmentSlotButtons) {
            Image i = btn.GetComponent<Image>();
            i.color = Color.white;
            i.sprite = equipmentTypeToSprite[tagToEquipmentType[btn.tag]];
        }
    }

    private void OpenEquipmentManager(TroopBase allyTroop) {
        Debug.Log("Open EquipmentManager for troop " + allyTroop.name);
        SetTroopInfo(allyTroop);
        // Load the equipment for the selected troop
        ClearSelectedEquipment();
        equipmentWindowManager.gameObject.SetActive(true);
        equipmentItemsWindow.gameObject.SetActive(true);
        resetEquipmentButton.gameObject.SetActive(true);
        LoadEquipmentManager(allyTroop);
    }

    private void CloseEquipmentManager() {
        Debug.Log("Close EquipmentManager");
        EquipmentTypeSelected = EquipmentType.None;
        SetTypeText();
        SetTroopInfo(null);
        equipmentWindowManager.gameObject.SetActive(false);
        equipmentItemsWindow.gameObject.SetActive(false);
        resetEquipmentButton.gameObject.SetActive(false);
    }

    private void LoadEquipmentManager(TroopBase troop) {
        if (troop == null) return;
        if (troopEquipmentMemory.ContainsKey(troop)) {
            this.troopEquipmentMemory[troop] = troop.equippedItems;
        } else this.troopEquipmentMemory.Add(troop, new());
        // Update equipment items to show the correct icons and slots to show equipped items
        UpdateEquipmentSelected();
    }

    private void UpdateEquipmentSelected() {
        // For each equipment button, check if the troop has this equipment
        foreach (var ebb in this.equipmentBtnBehaviors) {
            if (ebb.equipmentObject == null) continue;
            if (this.SelectedTroopHasEquipment(ebb.equipmentObject)) {
                ebb.SetColor(selectedIconColor);
                // Find slot for this equipment and assign it to the slot
                AssignEquippedSlot(ebb.equipmentObject.EquipmentType, ebb.equipmentObject);
            } else {
                ebb.SetColor(Color.white);
            }
        }
    }

    public bool SelectedTroopHasEquipment(IEquipment equipment) {
        TroopBase troop = this.GetSelectedTroop;
        if (troop == null || equipment == null) return false;
        if (troopEquipmentMemory.ContainsKey(troop)) {
            return troopEquipmentMemory[troop].Contains(equipment);
        }
        return false;
    }
    public void SetEquipmentsFromShop(Dictionary<EquipmentBase, int> equipmentToCount) {
        this.equipmentBtnBehaviors.ForEach(ebb => ebb.ResetEquipment());
        // Start filling equipment slots
        int ebIndex = 0;
        foreach (var kvp in equipmentToCount) {
            if (ebIndex >= this.equipmentBtnBehaviors.Count) break;
            this.equipmentBtnBehaviors[ebIndex++].SetEquipment(kvp.Key, kvp.Value);
        }
        Debug.Log("Bought " + equipmentToCount.Count + " equipment items and populated " + ebIndex + " equipment slots");
    }
}
