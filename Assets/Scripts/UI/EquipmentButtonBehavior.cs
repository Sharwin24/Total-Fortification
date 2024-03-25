using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EquipmentButtonBehavior : MonoBehaviour {
    public EquipmentBase equipmentObject;
    /// <summary>
    /// Number of these equipments the player has of this object
    /// </summary>
    public int count;

    private bool isEquipped = false;
    private Color selectedColor = new(0, 1, 0, 0.5f);
    private DeploymentManager deploymentManager;

    void Start() {
        deploymentManager = FindObjectOfType<DeploymentManager>();
        // Assign the button component OnClick handler
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnEquipmentButtonClicked);
    }

    void Update() {
        DisplayCount();
        if (deploymentManager != null && equipmentObject != null) {
            if (deploymentManager.GetEquipmentTypeSelected() == equipmentObject.EquipmentType) {
                GetComponent<Image>().color = Color.white;
            } else {
                GetComponent<Image>().color = Color.red;
            }
        }
    }

    private void OnEnable() {
        // Assign the image to the icon inside the equipmentObject
        if (equipmentObject == null) return;
        if (TryGetComponent<Image>(out var i)) i.sprite = equipmentObject.EquipmentIcon;
    }

    private void DisplayCount() {
        // Get this button's text component and assign it to the count
    }


    private void OnEquipmentButtonClicked() {
        Debug.Log("EquipmentButtonBehavior.OnEquipmentButtonClicked " + equipmentObject.EquipmentName);
        // Equip to Troop and set icon in equipment slot
        TroopBase troop = deploymentManager.GetSelectedTroop;
        if (troop == null) return;
        // If the troop already has this item equipped, then clicking this button should remove it
        if (troop.equippedItems.Contains(this.equipmentObject)) {
            troop.RemoveItem(this.equipmentObject.EquipmentType);
            this.count++;
            Debug.Log("Removed " + this.equipmentObject.EquipmentName + " from " + troop.name);
        } else {
            if (this.count == 0) {
                Debug.LogWarning("Cannot equip " + this.equipmentObject.name + ", none remaining");
                return;
            }
            troop.EquipItem(this.equipmentObject);
            this.count--;
            Debug.Log("Equipped " + this.equipmentObject.EquipmentName + " to " + troop.name);
        }
    }

}
