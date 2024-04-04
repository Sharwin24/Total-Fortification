using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static bool actionDone = false; // need to be updated when player/enemy completes combat action.
    public enum GameState
    {
        SHOP, DEPLOYMENT, COMBAT, END
    }

    public GameState gameState;
    public string nextLevel;
    public GameObject enemyManagement;
    public GameObject playerManagement;
    public Button enterBattleButton;
    public Button shopCloseButton;
    public TextMeshProUGUI levelMessage;
    public TextMeshProUGUI turnMessage;
    public TextMeshProUGUI scoreMessage;
    public TextMeshProUGUI scoreMultiplierMessage;
    public ScoreManager scoreManager;
    public GameObject shopUI;
    public GameObject deploymentUI;
    public GameObject combatUI;
    public AudioClip deploymentMusic;
    public AudioClip combatMusic;
    public int enemyCount = 0;
    public int allyCount = 0;
    private AudioSource cameraAudioSource;
    private PriorityQueue<GameObject> troopQueue;


    // Feature flags
    public bool skipDeployment = false;

    private int scoreMultiplier;

    void Start()
    {

        Initialize();

        DisplayUI(gameState);
        AssignUIComponents();

        enterBattleButton.onClick.AddListener(TaskOnClick);

        StartCoroutine(TakeTurnsCoroutine());
    }

    private void InitializeTroops()
    {
        print("Level Manager initializing");
        TroopBase[] allTroops = FindObjectsOfType<TroopBase>();
        enemyCount = 0;
        allyCount = 0;
        troopQueue = new PriorityQueue<GameObject>();

        for (int i = 0; i < allTroops.Length; i++)
        {
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

    void AssignUIComponents()
    {
        if (deploymentUI == null)
        {
            deploymentUI = GameObject.FindWithTag("DeploymentUI");
        }
        if (combatUI == null)
        {
            combatUI = GameObject.FindWithTag("CombatUI");
        }
        if (enterBattleButton == null)
        {
            enterBattleButton = GameObject.FindWithTag("EnterBattleButton").GetComponent<Button>();
        }
        if (shopCloseButton == null)
        {
            shopCloseButton = GameObject.FindWithTag("ShopCloseButton").GetComponent<Button>();
        }
        if (shopUI == null)
        {
            shopUI = GameObject.FindWithTag("ShopUI");
        }
        shopCloseButton.onClick.AddListener(ShopFinishOnClick);
        scoreManager = ScoreManager.Instance;
        //Display the user setting.
        scoreMultiplier = scoreManager.GetScoreMultiplier();
        StartCoroutine(TakeTurnsCoroutine());
    }

    IEnumerator TakeTurnsCoroutine()
    {

        print("Starting Game State: " + gameState);

        if (!skipDeployment)
        {
            print("Deployment Phase Started");
            yield return AwaitDeploymentCompletion();
        }
        print("Deployment Phase Skipped");
        yield return StartCombat();

        yield return new WaitForSeconds(0);
    }

    IEnumerator AwaitDeploymentCompletion()
    {

        print("Awaiting Deployment Completion");

        PlayMusic(deploymentMusic);
        DisplayUI(gameState);
        while (gameState == GameState.SHOP)
        {

            yield return new WaitForSeconds(2);
        }
        DisplayUI(gameState);
        // Start in Deployment Phase and when button is triggered, switch to Combat Phase
        // DisplayUI(gameState);
        gameState = GameState.DEPLOYMENT;
        // Allow Deployment Phase to run
        // Squares can be clicked and troops can be selected from inventory

        while (gameState == GameState.DEPLOYMENT)
        {
            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator StartCombat()
    {
        print("Starting Combat");

        gameState = GameState.COMBAT;
        DisplayUI(gameState);
        PlayMusic(combatMusic);

        EnemyBehavior enemyBehavior = enemyManagement.GetComponent<EnemyBehavior>();
        PlayerBehavior playerBehavior = playerManagement.GetComponent<PlayerBehavior>();
        int turnCount = 1;
        turnMessage.text = "Turn: " + turnCount;
        scoreMessage.text = "Score: " + scoreManager.GetScore();
        scoreMultiplierMessage.text = "Score Multiplier: " + scoreManager.GetScoreMultiplier() + "x";
        InitializeTroops();
        int troopsInTurn = troopQueue.Count;

        troopQueue.PrintElements();
        while (gameState == GameState.COMBAT && !troopQueue.IsEmpty())
        {

            print("Troop Queue Length: " + troopQueue.Count);
            troopQueue.PrintElements();

            GameObject troopGameObject = troopQueue.Dequeue();
            print("Attacking Troop: " + troopGameObject);

            scoreMessage.text = "Score: " + scoreManager.GetScore();

            if (troopGameObject == null)
            {
                troopsInTurn--;
                continue;
            }

            TroopBase currentTroop = troopGameObject.GetComponent<TroopBase>();

            if (currentTroop.tag == "Ally")
            {
                print(playerBehavior);
                StartCoroutine(playerBehavior.TakeAction(troopGameObject));
            }
            else if (currentTroop.tag == "Enemy")
            {
                StartCoroutine(enemyBehavior.TakeAction(troopGameObject));
            }
            else
            {
                throw new ArgumentException("Troop tag no recognized. Given: " + currentTroop.tag);
            }

            while (!actionDone)
            {
                // Wait for action to complete...
                yield return new WaitForSeconds(2);
            }
            actionDone = false;

            troopQueue.Enqueue(troopGameObject, -1 * currentTroop.Speed);
            troopsInTurn--;
            if (troopsInTurn <= 0)
            {
                turnCount++;
                turnMessage.text = "Turn: " + turnCount;
                troopsInTurn = troopQueue.Count; // Reset the counter for the next cycle of turns
                troopQueue.Reverse();
            }

            // If allycont or enemycount is 0, end the level
            if (this.allyCount == 0 || this.enemyCount == 0) this.gameState = GameState.END;
        }
        EndLevel();

        print("Combat Ended");


    }

    void LoadNextLevel()
    {
        if (nextLevel != null)
        {
            // Save the current level and score
            PlayerPrefs.SetString("CurrentLevel", nextLevel);
            PlayerPrefs.SetInt("CurrentScore", ScoreManager.Instance.GetScore());
            PlayerPrefs.Save();
            SceneManager.LoadScene(nextLevel);
        }
    }

    void ReloadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void DisplayUI(GameState gameState)
    {
        if (gameState == GameState.SHOP)
        {
            shopUI.SetActive(true);
            deploymentUI.SetActive(false);
            combatUI.SetActive(false);
        }
        else if (gameState == GameState.DEPLOYMENT)
        {
            combatUI.SetActive(false);
            deploymentUI.SetActive(true);
            shopUI.SetActive(false);
        }
        else if (gameState == GameState.COMBAT)
        {
            deploymentUI.SetActive(false);
            combatUI.SetActive(true);
            shopUI.SetActive(false);
        }
        else
        {
            deploymentUI.SetActive(false);
            combatUI.SetActive(false);
            shopUI.SetActive(false);
        }
    }

    public void TaskOnClick()
    {
        gameState = GameState.COMBAT;
    }

    public void ShopFinishOnClick()
    {
        gameState = GameState.DEPLOYMENT;
        Debug.Log("Player Purchased Following Equipments: " +
        shopUI.GetComponent<ShopManager>().printEquipmentList());
    }

    void Initialize()
    {
        levelMessage.text = "";
        turnMessage.text = "";
        scoreMessage.text = "";
        actionDone = false;
        if (!skipDeployment)
        {
            //Change to shop here
            gameState = GameState.SHOP;
        }
        cameraAudioSource = Camera.main.transform.Find("BackgroundMusic").GetComponent<AudioSource>();
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        cameraAudioSource.volume = volume;
    }
    public void PlayMusic(AudioClip clip)
    {
        cameraAudioSource.Stop();
        cameraAudioSource.clip = clip;
        cameraAudioSource.Play();
    }

    public void EndLevel()
    {
        print("Enemy Count: " + enemyCount);
        print("Aly Count: " + allyCount);
        if (enemyCount == 0)
        {
            levelMessage.text = "You win!";
            Invoke("LoadNextLevel", 3);
        }
        else
        {
            levelMessage.text = "You lost!";
            Invoke("ReloadCurrentLevel", 3);
        }
    }


}

