using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelManager : MonoBehaviour {

    public GameObject playerManagement;
    public GameObject enemyManagement;
    public enum GameState { COMBAT, END }
    public GameState gameState;
    PriorityQueue<GameObject> troopQueue;
    public int enemyCount;
    public int allyCount;
    public static bool actionDone = false;

    
    void Start() {
        Initialize();
        StartCoroutine(TakeTurnsCoroutine());
    }

    void Initialize() {
        gameState = GameState.COMBAT;
        troopQueue = new PriorityQueue<GameObject>();
        enemyCount = 0;
        allyCount = 0;
        InitializeTroops();
    }

    private void InitializeTroops() {
        TroopBase[] allTroops = FindObjectsOfType<TroopBase>();

        for (int i = 0; i < allTroops.Length; i++) {
            var troop = allTroops[i];
            troopQueue.Enqueue(troop.gameObject, troop.Speed);
            if (troop.gameObject.tag == "Enemy")
            {
                enemyCount++;
            }
            else if (troop.gameObject.tag == "Ally")
            {
                allyCount++;
            }
        }
    }


    IEnumerator TakeTurnsCoroutine() {

        int turnCount = 1;
        int troopsInTurn = troopQueue.Count;

        EnemyBehavior enemyBehavior = enemyManagement.GetComponent<EnemyBehavior>();
        PlayerBehavior playerBehavior = playerManagement.GetComponent<PlayerBehavior>();

        while (gameState != GameState.END) {
            GameObject troopGameObject = troopQueue.Dequeue();

            // Unity fake null logic - cannot check for null directly in list
            if (troopGameObject == null) {
                troopsInTurn--;
                continue;
            }

            TroopBase currentTroop = troopGameObject.GetComponent<TroopBase>();

            print("Current Troop Tag - " + currentTroop.tag);

            // Take action based on tag
            if (currentTroop.tag == "Ally") {
                StartCoroutine(playerBehavior.TakeAction(troopGameObject));
            } else if (currentTroop.tag == "Enemy") {
                StartCoroutine(enemyBehavior.TakeAction(troopGameObject));
            } else {
                throw new ArgumentException("Troop tag no recognized. Given: " + currentTroop.tag);
            }

            // Wait for action/animation to end.
            yield return new WaitUntil(() => actionDone);   

            actionDone = false;

            // Re-enqueue troop with low priority
            troopQueue.Enqueue(troopGameObject, -1 * currentTroop.Speed);

            troopsInTurn--;

            // Flip Queue if all troops have taken action
            if (troopsInTurn <= 0) {
                turnCount++;
                troopsInTurn = troopQueue.Count; 
                troopQueue.Reverse();
            }

            // If allycont or enemycount is 0, end the level
            if (this.allyCount == 0 || this.enemyCount == 0) this.gameState = GameState.END;

        }

        print("Combat Ended");
        EndGame();
    }

    void EndGame() {
        if (enemyCount == 0) {
            print("You Win!");
            Invoke("LoadNextLevel", 3);
        } else {
            print("You Lost!");
        }
    }
}
