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

    private List<GameObject> allies;

    private readonly Dictionary<string, int> tagToTroopIndex = new Dictionary<string, int> {
        { "TroopBtn1", 0 },
        { "TroopBtn2", 1 },
        { "TroopBtn3", 2 },
        { "TroopBtn4", 3 }
    };

    private void Awake() {
    }

    // Start is called before the first frame update
    void Start() {
        // Collect all GameObjects with Ally tag
        allies = GameObject.FindGameObjectsWithTag("Ally").ToList();

        foreach (var icon in troopIcons) {
            int index = tagToTroopIndex[icon.tag];
            if (troopButtons[index] == null) troopButtons[index] = icon.GetComponent<Button>();
            troopButtons[index].onClick.AddListener(() => OnTroopButtonClicked(icon));
        }
    }

    // Update is called once per frame
    void Update() {
    }

    private void SelectedTroop(Image icon) {
        foreach (var i in troopIcons) {
            if (i == icon) {
                // Change to green with transparency so image can be seen
                i.color = new Color(0, 1, 0, 0.5f);
                break;
            }
        }
    }

    private void DeselectAll() {
        foreach (var i in troopIcons) {
            i.color = Color.white;
        }
    }

    private void OpenEquipmentManager(GameObject allyTroop) {
        print("Open EquipmentManager for troop " + allyTroop.name);
    }


    public void OnTroopButtonClicked(Image icon) {
        DeselectAll();
        SelectedTroop(icon);
        int index = tagToTroopIndex[icon.tag];
        if (index >= allies.Count) return;
        var troop = allies[index];
        print("Selected troop: " + troop.name); // Troop should visually appear selected somehow
        OpenEquipmentManager(troop);
    }
}
