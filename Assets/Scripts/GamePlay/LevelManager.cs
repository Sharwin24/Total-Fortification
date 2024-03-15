using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static bool actionDone = false; // need to be updated when player/enemy completes combat action.
    public enum GameState {
        DEPLOYMENT, COMBAT, END
    }

    public GameState gameState;
    public string nextLevel;
    public GameObject enemyManagement;
    public Button enterBattleButton;
    public TextMeshProUGUI levelMessage;
    public TextMeshProUGUI turnMessage;
    // public GameObject deploymentUI;
    // public GameObject combatUI;

    private PriorityQueue<GameObject> troopQueue = new PriorityQueue<GameObject>();

    void Start() {
        levelMessage.text = "";
        turnMessage.text = "";
        actionDone = false;
        // gameState = GameState.DEPLOYMENT; // Initial setup for demonstration purposes
        // if (deploymentUI == null) {
        //     deploymentUI = GameObject.FindWithTag("DeploymentUI");
        // }
        // if (combatUI == null) {
        //     combatUI = GameObject.FindWithTag("CombatUI");
        // }
        // if (enterBattleButton == null) {
        //     enterBattleButton = GameObject.FindWithTag("EnterBattleButton").GetComponent<Button>();
        // }
        // enterBattleButton.onClick.AddListener(TaskOnClick);
        gameState = GameState.COMBAT;
        InitializeTroops();
        StartCoroutine(TakeTurnsCoroutine());
    }

    private void InitializeTroops() {
        print("Level Manager initializing");
        TroopBase[] allTroops = FindObjectsOfType<TroopBase>();

        foreach (var troop in allTroops) {
            troopQueue.Enqueue(troop.gameObject, troop.Speed);
        }
    }

    IEnumerator TakeTurnsCoroutine() {
        print("TakeTurnsCoroutine triggered");

        // Start in Deployment Phase and when button is triggered, switch to Combat Phase
        // DisplayUI(gameState);
        // while (gameState == GameState.DEPLOYMENT) {

        //     // Allow Deployment Phase to run
        //     // Squares can be clicked and troops can be selected from inventory

        //     yield return new WaitForSeconds(2);
        // }
        EnemyBehavior enemyBehavior = enemyManagement.GetComponent<EnemyBehavior>();
        int turnCount = 1;
        int troopsInTurn = troopQueue.Count;
        turnMessage.text = "Turn: " + turnCount;

        // DisplayUI(gameState);
        while (gameState == GameState.COMBAT && !troopQueue.IsEmpty()) {
            print("Loop entered");

            // Add Game End Logic Here

            GameObject troopGameObject = troopQueue.Dequeue();
            print(troopGameObject);

            if (troopGameObject == null) {
                troopsInTurn--;
                continue;
            }

            TroopBase currentTroop = troopGameObject.GetComponent<TroopBase>();

            print(troopGameObject.tag);

            if (currentTroop.tag == "Ally") {
                PlayerBehavior.TakeAction(troopGameObject);

            } else if (currentTroop.tag == "Enemy") {
                print("Jumping to EnemyBehavior.TakeAction()");
                StartCoroutine(enemyBehavior.TakeAction(troopGameObject));
            } else {
                throw new ArgumentException("Troop tag no recognized. Given: " + currentTroop.tag);
            }

            while (!actionDone) {
                // Wait for action to complete...
                yield return new WaitForSeconds(2);
            }
            actionDone = false;

            troopQueue.Enqueue(troopGameObject, currentTroop.Speed);

            troopsInTurn--;
            if (troopsInTurn <= 0) {
                turnCount++;
                turnMessage.text = "Turn: " + turnCount;
                troopsInTurn = troopQueue.Count; // Reset the counter for the next cycle of turns
            }
            
        }

        if (gameState == GameState.COMBAT) {
            gameState = GameState.END;
            // Handle end of combat
        }
        yield return new WaitForSeconds(0);
    }

    // void DisplayUI(GameState gameState) {
    //     if (gameState == GameState.DEPLOYMENT) {
    //         combatUI.SetActive(false);
    //         deploymentUI.SetActive(true);
    //     } else if (gameState == GameState.COMBAT) {
    //         deploymentUI.SetActive(false);
    //         combatUI.SetActive(true);
    //     }
    // }

    public void TaskOnClick() {
        gameState = GameState.COMBAT;
    }



}
