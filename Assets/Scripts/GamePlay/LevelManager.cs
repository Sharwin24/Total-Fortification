using System;
using System.Collections;
using UnityEngine.UI;
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

    private PriorityQueue<GameObject> troopQueue = new PriorityQueue<GameObject>();

    private GameObject deploymentUI;
    private GameObject combatUI;

    void Start() {
        actionDone = false;
        gameState = GameState.COMBAT; // Initial setup for demonstration purposes
        if (deploymentUI == null) {
            deploymentUI = GameObject.FindWithTag("DeploymentUI");
        }
        if (combatUI == null) {
            combatUI = GameObject.FindWithTag("CombatUI");
        }
        if (enterBattleButton == null) {
            enterBattleButton = GameObject.FindWithTag("EnterBattleButton").GetComponent<Button>();
        }
        enterBattleButton.onClick.AddListener(TaskOnClick);
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
        DisplayUI(gameState);
        while (gameState == GameState.DEPLOYMENT) {

            // Allow Deployment Phase to run
            // Squares can be clicked and troops can be selected from inventory

            yield return new WaitForSeconds(2);
        }

        EnemyBehavior enemyBehavior = enemyManagement.GetComponent<EnemyBehavior>();
        DisplayUI(gameState);
        while (gameState == GameState.COMBAT && !troopQueue.IsEmpty()) {

            print("Loop entered");

            GameObject troopGameObject = troopQueue.Dequeue();
            print(troopGameObject);

            if (troopGameObject == null) {
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

        }

        if (gameState == GameState.COMBAT) {
            gameState = GameState.END;
            // Handle end of combat
        }
    }

    void DisplayUI(GameState gameState) {
        if (gameState == GameState.DEPLOYMENT) {
            combatUI.SetActive(false);
            deploymentUI.SetActive(true);
        } else if (gameState == GameState.COMBAT) {
            deploymentUI.SetActive(false);
            combatUI.SetActive(true);
        }
    }

    public void TaskOnClick() {
        gameState = GameState.COMBAT;
    }



}
