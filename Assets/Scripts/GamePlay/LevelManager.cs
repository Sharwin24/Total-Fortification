using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static bool actionDone = false; // need to be updated when player/enemy completes combat action.
    public enum GameState {
        DEPLOYMENT, COMBAT, END
    }

    public GameState gameState;
    public string nextLevel;
    public GameObject enemyManagement;

    private PriorityQueue<GameObject> troopQueue = new PriorityQueue<GameObject>();

    void Start() {
        actionDone = false;
        gameState = GameState.COMBAT; // Initial setup for demonstration purposes
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

        EnemyBehavior enemyBehavior = enemyManagement.GetComponent<EnemyBehavior>();

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



}
