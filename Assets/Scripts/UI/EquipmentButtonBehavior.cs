using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EquipmentButtonBehavior : MonoBehaviour {
    public EquipmentBase equipmentObject;

    private bool isEquipped = false;
    private Color selectedColor = new(0, 1, 0, 0.5f);
    private DeploymentManager deploymentManager;
    // Start is called before the first frame update
    void Start() {
        deploymentManager = FindObjectOfType<DeploymentManager>();
        // Assign the button component OnClick handler
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnEquipmentButtonClicked);
    }

    // Update is called once per frame
    void Update() {
        if (deploymentManager == null || equipmentObject == null) return;
        if (deploymentManager.GetEquipmentTypeSelected() == equipmentObject.EquipmentType) {
            GetComponent<Image>().color = Color.white;
        } else {
            GetComponent<Image>().color = Color.red;
        }
    }

    void OnEnable() {
        // Assign the image to the icon inside the equipmentObject
        if (equipmentObject == null) return;
        if (TryGetComponent<Image>(out var i)) i.sprite = equipmentObject.EquipmentIcon;
    }


    void OnEquipmentButtonClicked() {
        Debug.Log("EquipmentButtonBehavior.OnEquipmentButtonClicked " + equipmentObject.EquipmentName);
        // Equip to Troop and set icon in equipment slot
    }

}
