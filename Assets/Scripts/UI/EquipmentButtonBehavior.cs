using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EquipmentButtonBehavior : MonoBehaviour {
    public EquipmentBase equipmentObject;
    /// <summary>
    /// Number of these equipments the player has of this object
    /// </summary>
    public int count = 0;

    private TextMeshProUGUI countText;
    private Color selectedColor = new(0, 1, 0, 0.5f);
    private DeploymentManager deploymentManager;
    private Button resetEquipmentButton;


    private void Awake() {
        if (!TryGetComponent<DeploymentManager>(out deploymentManager)) deploymentManager = GameObject.FindGameObjectWithTag("DeploymentManager").GetComponent<DeploymentManager>();
    }

    void Start() {
        if (deploymentManager != null && equipmentObject != null) {
            if (deploymentManager.EquipmentInventory.ContainsKey(equipmentObject)) count = deploymentManager.EquipmentInventory[equipmentObject];
            else deploymentManager.EquipmentInventory[equipmentObject] = count;
        } else return;
        // Assign the button component OnClick handler
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnEquipmentButtonClicked);
        resetEquipmentButton = GameObject.FindGameObjectWithTag("ResetEquipmentButton").GetComponent<Button>();
        resetEquipmentButton.onClick.AddListener(() => OnResetEquipmentButtonClicked());
        countText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update() {
        DisplayCount();
    }

    public void EnableByType(EquipmentType type) {
        if (type == EquipmentType.None || this.equipmentObject == null) this.gameObject.SetActive(true);
        else if (type != this.equipmentObject.EquipmentType) this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }

    private void OnEnable() {
        // Assign the image to the icon inside the equipmentObject
        if (equipmentObject == null) return;
        if (TryGetComponent<Image>(out var i)) i.sprite = equipmentObject.EquipmentIcon;
        if (deploymentManager.SelectedTroopHasEquipment(this.equipmentObject)) SetColor(selectedColor);
        else SetColor(Color.white);
    }

    private void SetColor(Color color) {
        GetComponent<Image>().color = color;
    }

    private void DisplayCount() {
        if (countText != null) countText.text = count.ToString();
    }

    private void OnEquipmentButtonClicked() {
        Debug.Log("EquipmentButtonBehavior.OnEquipmentButtonClicked " + equipmentObject.EquipmentName);
        // Equip to Troop and set icon in equipment slot
        TroopBase troop = deploymentManager.GetSelectedTroop;
        deploymentManager.SetTroopInfo(troop);
        if (troop == null) return;
        if (deploymentManager.GetEquipmentTypeSelected() != equipmentObject.EquipmentType) {
            Debug.LogWarning("Cannot equip " + equipmentObject + " to " + equipmentObject.EquipmentType);
            return;
        }
        // If the troop already has this item equipped, then clicking this button should remove it
        if (troop.equippedItems.Contains(this.equipmentObject)) {
            troop.RemoveItem(this.equipmentObject.EquipmentType);
            deploymentManager.SetTroopInfo(troop);
            deploymentManager.ClearEquippedSlot(this.equipmentObject.EquipmentType);
            SetColor(Color.white);
            this.count++;
            Debug.Log("Removed " + this.equipmentObject.EquipmentName + " from " + troop.name);
        } else {
            if (this.count == 0) {
                Debug.LogWarning("Cannot equip " + this.equipmentObject.name + ", none remaining");
                return;
            }
            troop.EquipItem(this.equipmentObject);
            deploymentManager.SetTroopInfo(troop);
            deploymentManager.AssignEquippedSlot(this.equipmentObject.EquipmentType, this.equipmentObject);
            SetColor(selectedColor);
            this.count--;
            Debug.Log("Equipped " + this.equipmentObject.EquipmentName + " to " + troop.name);
        }
    }

    private void OnResetEquipmentButtonClicked() {
        TroopBase troop = deploymentManager.GetSelectedTroop;
        deploymentManager.SetTroopInfo(troop);
        if (troop == null) return;
        troop.RemoveItem(equipmentObject.EquipmentType);
        count++;
    }

}
