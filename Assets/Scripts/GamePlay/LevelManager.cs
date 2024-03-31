using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public static bool actionDone = false; // need to be updated when player/enemy completes combat action.
    public enum GameState {
        DEPLOYMENT, COMBAT, END
    }

    public GameState gameState;
    public string nextLevel;
    public GameObject enemyManagement;
    public GameObject playerManagement;
    public Button enterBattleButton;
    public TextMeshProUGUI levelMessage;
    public TextMeshProUGUI turnMessage;
    public TextMeshProUGUI scoreMessage;
    public GameObject deploymentUI;
    public GameObject combatUI;
    public AudioClip deploymentMusic;
    public AudioClip combatMusic;
    private AudioSource cameraAudioSource;
    private PriorityQueue<GameObject> troopQueue = new PriorityQueue<GameObject>();

    // Feature flags
    public bool skipDeployment = false;

    void Start() {

        Initialize();

        DisplayUI(gameState);
        AssignUIComponents();

        enterBattleButton.onClick.AddListener(TaskOnClick);

        StartCoroutine(TakeTurnsCoroutine());
    }

    private void InitializeTroops() {
        print("Level Manager initializing");
        TroopBase[] allTroops = FindObjectsOfType<TroopBase>();

        foreach (var troop in allTroops) {
            troopQueue.Enqueue(troop.gameObject, troop.Speed);
        }

    }

    void AssignUIComponents() {
        if (deploymentUI == null) {
            deploymentUI = GameObject.FindWithTag("DeploymentUI");
        }
        if (combatUI == null) {
            combatUI = GameObject.FindWithTag("CombatUI");
        }
        if (enterBattleButton == null) {
            enterBattleButton = GameObject.FindWithTag("EnterBattleButton").GetComponent<Button>();
        }
    }

    IEnumerator TakeTurnsCoroutine() {

        print("Starting Game State: " + gameState);
        
        if (!skipDeployment) {
            print("Deployment Phase Started");
            yield return AwaitDeploymentCompletion();
        }
        print("Deployment Phase Skipped");
        yield return StartCombat();

        yield return new WaitForSeconds(0);
    }

    IEnumerator AwaitDeploymentCompletion() {

        print("Awaiting Deployment Completion");

        PlayMusic(deploymentMusic);
        DisplayUI(gameState);

        while (gameState == GameState.DEPLOYMENT) {
            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator StartCombat() {
        print("Starting Combat");

        gameState = GameState.COMBAT;
        PlayMusic(combatMusic);

        EnemyBehavior enemyBehavior = enemyManagement.GetComponent<EnemyBehavior>();
        PlayerBehavior playerBehavior = playerManagement.GetComponent<PlayerBehavior>();
        int turnCount = 1;
        turnMessage.text = "Turn: " + turnCount;
        scoreMessage.text = "Score: " + playerBehavior.playerScore;
        InitializeTroops();
        int troopsInTurn = troopQueue.Count;
        DisplayUI(gameState);

        print("Troop Queue Length: " + troopQueue.Count);
        while (gameState == GameState.COMBAT && !troopQueue.IsEmpty()) {

            print("Troop Queue Length: " + troopQueue.Count);
            troopQueue.PrintElements();

            GameObject troopGameObject = troopQueue.Dequeue();
            print("Attacking Troop: " + troopGameObject);
            print("Attacking Troop Tag: " + troopGameObject.tag);

            scoreMessage.text = "Score: " + playerBehavior.playerScore;

            if (troopGameObject == null) {
                troopsInTurn--;
                continue;
            }

            TroopBase currentTroop = troopGameObject.GetComponent<TroopBase>();

            if (currentTroop.tag == "Ally") {
                print(playerBehavior);
                StartCoroutine(playerBehavior.TakeAction(troopGameObject));
            } else if (currentTroop.tag == "Enemy") {
                StartCoroutine(enemyBehavior.TakeAction(troopGameObject));
            } else {
                throw new ArgumentException("Troop tag no recognized. Given: " + currentTroop.tag);
            }

            while (!actionDone) {
                // Wait for action to complete...
                yield return new WaitForSeconds(2);
            }
            actionDone = false;

            troopQueue.Enqueue(troopGameObject, -1 * currentTroop.Speed);

            // Check Game End
            List<GameObject> allTroops = troopQueue.ToList();

            if (CheckIfTagExists(allTroops, "Enemy")) {
                levelMessage.text = "You win!";
                Invoke("LoadNextLevel", 3);
                break;
            } else if (CheckIfTagExists(allTroops, "Ally")) {
                levelMessage.text = "You lost!";
                Invoke("ReloadCurrentLevel", 3);
                break;
            }

            troopsInTurn--;
            if (troopsInTurn <= 0) {
                turnCount++;
                turnMessage.text = "Turn: " + turnCount;
                troopsInTurn = troopQueue.Count; // Reset the counter for the next cycle of turns
                troopQueue.Reverse();
            }
        }

        print("Combat Ended");

    }

    void LoadNextLevel() {
        if (nextLevel != null) {
            SceneManager.LoadScene(nextLevel);
        }
    }

    void ReloadCurrentLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    bool CheckIfTagExists(List<GameObject> allTroops, string tag) {
        int count = 0;
        foreach (GameObject troop in allTroops) {
            if (troop != null && troop.tag == tag) {
                count++;
            }
        }

        return count == 0;
    }

    void Initialize() {
        levelMessage.text = "";
        turnMessage.text = "";
        scoreMessage.text = "";
        actionDone = false;
        if (!skipDeployment) {
            gameState = GameState.DEPLOYMENT;
        } 
        cameraAudioSource = Camera.main.transform.Find("BackgroundMusic").GetComponent<AudioSource>();
    }

    public int GetPlayerScore() {
        return playerManagement.GetComponent<PlayerBehavior>().playerScore;
    }
    public void PlayMusic(AudioClip clip) {
        cameraAudioSource.Stop();
        cameraAudioSource.clip = clip;
        cameraAudioSource.Play();
    }




}

