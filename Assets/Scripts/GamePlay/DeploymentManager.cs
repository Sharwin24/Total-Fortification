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

    private List<GameObject> allies;

    private readonly Dictionary<string, int> tagToTroopIndex = new Dictionary<string, int> {
        { "TroopBtn1", 0 },
        { "TroopBtn2", 1 },
        { "TroopBtn3", 2 },
        { "TroopBtn4", 3 }
    };

    private Dictionary<GameObject, List<bool>> equipmentSelected = new Dictionary<GameObject, List<bool>>();

    private bool equipmentManagerOpen = false;
    private Color selectedColor = new(0, 1, 0, 0.5f);
    private Image selectedTroopIcon;

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
        // Collect all GameObjects with Ally tag
        allies = GameObject.FindGameObjectsWithTag("Ally").ToList();
    }

    // Update is called once per frame
    void Update() {
        if (equipmentManagerOpen) {
            // Display equipment manager
            equipmentManager.gameObject.SetActive(true);
        } else {
            // Hide equipment manager
            // Save selected equipment for that ally
            SaveSelectedEquipment();
            ClearSelectedEquipment();
            equipmentManager.gameObject.SetActive(false);
        }
    }

    private void SelectedTroop(Image icon) {
        Image foundIcon = troopIcons.Find(i => i == icon);
        // If the icon is already selected, unselect it
        if (foundIcon.color == selectedColor) {
            foundIcon.color = Color.white;
        } else {
            // Change to green with transparency so image can be seen
            foundIcon.color = selectedColor;
        }
        selectedTroopIcon = foundIcon;
    }

    private void DeselectAll() {
        foreach (var i in troopIcons) {
            i.color = Color.white;
        }
        selectedTroopIcon = null;
    }

    private void OpenEquipmentManager(GameObject allyTroop) {
        print("Open EquipmentManager for troop " + allyTroop.name);
        equipmentManagerOpen = true;
        // Load the equipment for the selected troop
    }

    private GameObject GetTroopFromIcon(Image troopIcon) {
        int index = tagToTroopIndex[troopIcon.tag];
        if (index >= allies.Count) return null;
        return allies[index];
    }


    public void OnTroopButtonClicked(Image icon) {
        // If the icon is already selected, unselect it and close the equipment manager
        if (icon.color == selectedColor) {
            DeselectAll();
            equipmentManagerOpen = false;
            return;
        }
        DeselectAll();
        SelectedTroop(icon);
        var troop = GetTroopFromIcon(icon);
        if (troop == null) return;
        print("Selected troop: " + troop.name); // Troop should visually appear selected somehow
        OpenEquipmentManager(troop);
    }

    private void SelectEquipment(Image icon) {
        Image foundIcon = equipmentIcons.Find(i => i == icon);
        // If the icon is already selected, unselect it
        if (foundIcon.color == selectedColor) {
            foundIcon.color = Color.white;
        } else {
            // Change to green with transparency so image can be seen
            foundIcon.color = selectedColor;
        }
        // Save the selected equipment for the current ally
        var troop = GetTroopFromIcon(selectedTroopIcon);
        if (troop == null) return;
        var equipmentIndex = equipmentIcons.FindIndex(i => i == icon);
        if (!equipmentSelected.ContainsKey(troop)) {
            equipmentSelected[troop] = new List<bool> { false, false, false, false };
        }
        equipmentSelected[troop][equipmentIndex] = foundIcon.color == selectedColor;
    }

    private void SaveSelectedEquipment() {
        // The current selected equipments should be saved to the corresponding ally
        var troop = GetTroopFromIcon(selectedTroopIcon);
        if (troop == null) return;
        // Update dictionary
        if (!equipmentSelected.ContainsKey(troop)) {
            equipmentSelected[troop] = new List<bool> { false, false, false, false };
        }
    }

    private void ClearSelectedEquipment() {
        foreach (var i in equipmentIcons) {
            i.color = Color.white;
        }
    }

    public void OnEquipmentButtonClicked(Image icon) {
        print("Selected equipment: " + icon.name);
        SelectEquipment(icon);
    }
}
