using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentManager : MonoBehaviour {
    private const int numTroops = 4;
    public Image[] troopIcons = new Image[numTroops];
    public GameObject[] troopPrefabs = new GameObject[numTroops];

    private readonly Dictionary<string, int> tagToTroopIndex = new Dictionary<string, int> {
        {"Troop1Btn", 0},
        {"Troop2Btn", 1},
        {"Troop3Btn", 2},
        {"Troop4Btn", 3}
    };

    private int[] troopCounts = new int[numTroops];
    private Text[] troopCountTexts = new Text[numTroops];
    private Button[] troopButtons = new Button[numTroops];

    private void Awake() {
        for (int i = 0; i < numTroops; i++) {
            troopCounts[i] = 0;
        }
    }

    // Start is called before the first frame update
    void Start() {
        foreach (var icon in troopIcons) {
            int index = tagToTroopIndex[icon.tag];
            troopCountTexts[index] = icon.GetComponentInChildren<Text>();
            troopButtons[index] = icon.GetComponent<Button>();
            troopButtons[index].onClick.AddListener(() => OnTroopButtonClicked(index));
        }
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < numTroops; i++) {
            troopCountTexts[i].text = troopCounts[i].ToString();
        }
    }

    public void SetTroopCounts(int troopIndex, int count) {
        if (troopIndex < 0 || troopIndex >= numTroops) {
            Debug.LogError("Invalid troop index: " + troopIndex);
            return;
        }
        troopCounts[troopIndex] = count;
    }


    public void OnTroopButtonClicked(int troopIndex) {
        print("Button " + troopIndex + " clicked");
        if (troopCounts[troopIndex] > 0) {
            troopCounts[troopIndex]--;
            // Deploy troop
            //GameObject troop = Instantiate(troopPrefabs[troopIndex]);
        }
    }
}
