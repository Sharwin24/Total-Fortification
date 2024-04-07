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
    private Color selectedIconColor = new(0, 1, 0, 0.5f);
    private DeploymentManager deploymentManager;
    private Button resetEquipmentButton;


    private void Awake() {
        if (!TryGetComponent<DeploymentManager>(out deploymentManager)) deploymentManager = GameObject.FindGameObjectWithTag("DeploymentManager").GetComponent<DeploymentManager>();
        if (deploymentManager == null) Debug.LogError("DeploymentManager not found");
    }

    void Start() {
        if (deploymentManager != null && equipmentObject != null) {
            if (deploymentManager.EquipmentInventory.ContainsKey(equipmentObject)) count = deploymentManager.EquipmentInventory[equipmentObject];
            else deploymentManager.EquipmentInventory[equipmentObject] = count;
        } else if (equipmentObject == null) {
            count = 0;
        }
        // Assign the button component OnClick handler
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnEquipmentButtonClicked);
        resetEquipmentButton = GameObject.FindGameObjectWithTag("ResetEquipmentButton").GetComponent<Button>();
        resetEquipmentButton.onClick.AddListener(() => OnResetEquipmentButtonClicked());
        countText = GetComponentInChildren<TextMeshProUGUI>();
        countText.text = count.ToString();
    }

    void Update() {
        DisplayCount();
    }

    public void EnableByType(EquipmentType type) {
        if (this.equipmentObject == null) this.gameObject.SetActive(false);
        else if (type == EquipmentType.None) this.gameObject.SetActive(true);
        else if ((type == EquipmentType.LeftArm || type == EquipmentType.RightArm) && this.equipmentObject.EquipmentType == EquipmentType.TwoHanded) this.gameObject.SetActive(true);
        else if (type != this.equipmentObject.EquipmentType) this.gameObject.SetActive(false);
        else this.gameObject.SetActive(true);
    }

    private void OnEnable() {
        // Assign the image to the icon inside the equipmentObject
        if (equipmentObject == null) return;
        if (TryGetComponent<Image>(out var i)) i.sprite = equipmentObject.EquipmentIcon;
        if (deploymentManager.SelectedTroopHasEquipment(this.equipmentObject)) SetColor(selectedIconColor);
        else SetColor(Color.white);
    }

    public void SetColor(Color color) {
        GetComponent<Image>().color = color;
    }

    private void DisplayCount() {
        if (countText != null) countText.text = count.ToString();
    }

    private void OnEquipmentButtonClicked() {
        if (this.equipmentObject == null) return;
        Debug.Log("EquipmentButtonBehavior.OnEquipmentButtonClicked " + equipmentObject.EquipmentName);
        // Equip to Troop and set icon in equipment slot
        TroopBase troop = deploymentManager.GetSelectedTroop;
        deploymentManager.SetTroopInfo(troop);
        if (troop == null) return;
        // If the troop already has this item equipped, then clicking this button should remove it
        if (troop.equippedItems.Contains(this.equipmentObject)) {
            bool removed = troop.RemoveItem(this.equipmentObject.EquipmentType);
            deploymentManager.SetTroopInfo(troop);
            if (!removed) return;
            deploymentManager.ClearEquippedSlot(this.equipmentObject.EquipmentType);
            SetColor(Color.white);
            this.count++;
            Debug.Log("Removed " + this.equipmentObject.EquipmentName + " from " + troop.name);
        } else {
            if (this.count == 0) {
                Debug.LogWarning("Cannot equip " + this.equipmentObject.name + ", none remaining");
                return;
            } else if (troop.equippedItems.Exists(e => e.EquipmentType == this.equipmentObject.EquipmentType)) {
                // If the troop has an item equipped in the body part where this item should go, don't equip it and show a warning
                Debug.LogWarning("Cannot equip " + this.equipmentObject.name + ", " + troop.name + " already has an item equipped in the " + this.equipmentObject.EquipmentType + " slot");
                return;
            }
            bool equipped = troop.EquipItem(this.equipmentObject);
            deploymentManager.SetTroopInfo(troop);
            if (!equipped) return;
            deploymentManager.AssignEquippedSlot(this.equipmentObject.EquipmentType, this.equipmentObject);
            SetColor(selectedIconColor);
            this.count--;
            Debug.Log("Equipped " + this.equipmentObject.EquipmentName + " to " + troop.name);
        }
    }

    private void OnResetEquipmentButtonClicked() {
        TroopBase troop = deploymentManager.GetSelectedTroop;
        deploymentManager.SetTroopInfo(troop);
        if (troop == null || equipmentObject == null) return;
        if (troop.RemoveItem(equipmentObject.EquipmentType)) {
            count++;
            SetColor(Color.white);
        }
        deploymentManager.ClearEquippedSlot(equipmentObject.EquipmentType);
    }

}
