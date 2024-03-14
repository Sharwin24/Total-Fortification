using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeploymentManager : MonoBehaviour
{
    private const int numTroops = 4;
    public Button[] troopButtons = new Button[numTroops];
    public int[] troopCounts = new int[numTroops];
    public GameObject[] troopPrefabs = new GameObject[numTroops];

    private readonly Dictionary<string, int> tagToTroopIndex = new Dictionary<string, int> {
        {"Troop1Btn", 0},
        {"Troop2Btn", 1},
        {"Troop3Btn", 2},
        {"Troop4Btn", 3}
    };

    private Text[] troopCountTexts = new Text[numTroops];

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numTroops; i++) {
            troopCountTexts[i] = troopButtons[i].GetComponentInChildren<Text>();
        }
        foreach (var button in troopButtons) {
            int index = tagToTroopIndex[button.tag];
            button.onClick.AddListener(() => OnTroopButtonClicked(index));
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < numTroops; i++) {
            troopCountTexts[i].text = troopCounts[i].ToString();
        }
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
