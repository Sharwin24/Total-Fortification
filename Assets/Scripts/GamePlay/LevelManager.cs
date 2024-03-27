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
    public ScoreManager scoreManager;
    public GameObject shopUI;
    public GameObject deploymentUI;
    public GameObject combatUI;
    public AudioClip deploymentMusic;
    public AudioClip combatMusic;
    private AudioSource cameraAudioSource;
    private PriorityQueue<GameObject> troopQueue = new PriorityQueue<GameObject>();



    void Start()
    {

        Initialize();
        if (shopUI == null)
        {
            shopUI = GameObject.FindWithTag("ShopUI");
        }
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

        enterBattleButton.onClick.AddListener(TaskOnClick);
        shopCloseButton.onClick.AddListener(ShopFinishOnClick);
        StartCoroutine(TakeTurnsCoroutine());
    }

    private void InitializeTroops()
    {
        print("Level Manager initializing");
        TroopBase[] allTroops = FindObjectsOfType<TroopBase>();

        foreach (var troop in allTroops)
        {
            troopQueue.Enqueue(troop.gameObject, troop.Speed);
        }

    }

    IEnumerator TakeTurnsCoroutine()
    {
        print("TakeTurnsCoroutine triggered");

        PlayMusic(deploymentMusic);

        DisplayUI(gameState);
        while (gameState == GameState.SHOP)
        {

            yield return new WaitForSeconds(2);
        }
        DisplayUI(gameState);
        // Start in Deployment Phase and when button is triggered, switch to Combat Phase
        // DisplayUI(gameState);
        while (gameState == GameState.DEPLOYMENT)
        {

            // Allow Deployment Phase to run
            // Squares can be clicked and troops can be selected from inventory

            yield return new WaitForSeconds(2);
        }

        gameState = GameState.COMBAT;
        PlayMusic(combatMusic);
        EnemyBehavior enemyBehavior = enemyManagement.GetComponent<EnemyBehavior>();
        PlayerBehavior playerBehavior = playerManagement.GetComponent<PlayerBehavior>();
        int turnCount = 1;
        int troopsInTurn = troopQueue.Count;
        turnMessage.text = "Turn: " + turnCount;
        scoreMessage.text = "Score: " + scoreManager.GetScore();
        InitializeTroops();
        DisplayUI(gameState);
        while (gameState == GameState.COMBAT && !troopQueue.IsEmpty())
        {

            GameObject troopGameObject = troopQueue.Dequeue();
            print(troopGameObject);
            scoreMessage.text = "Score: " + scoreManager.GetScore();

            if (troopGameObject == null)
            {
                Debug.Log("In Game Troop Length " + troopQueue.Count);
                troopsInTurn--;
                continue;
            }

            TroopBase currentTroop = troopGameObject.GetComponent<TroopBase>();

            print(troopGameObject.tag);
            Vector3 currentTroopPosition = troopGameObject.transform.position;

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

            // Check Game End
            List<GameObject> allTroops = troopQueue.ToList();

            if (CheckIfTagExists(allTroops, "Enemy"))
            {
                levelMessage.text = "You win!";
                Invoke("LoadNextLevel", 3);
                break;
            }
            else if (CheckIfTagExists(allTroops, "Ally"))
            {
                levelMessage.text = "You lost!";
                break;
            }

            troopsInTurn--;
            if (troopsInTurn <= 0)
            {
                turnCount++;
                turnMessage.text = "Turn: " + turnCount;
                troopsInTurn = troopQueue.Count; // Reset the counter for the next cycle of turns
                troopQueue.Reverse();
            }
        }


        yield return new WaitForSeconds(0);
    }

    void LoadNextLevel()
    {
        if (nextLevel != null)
        {
            SceneManager.LoadScene(nextLevel);
        }
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
    }

    public void TaskOnClick()
    {
        gameState = GameState.COMBAT;
    }

    public void ShopFinishOnClick()
    {
        gameState = GameState.DEPLOYMENT;
        Debug.Log("Shop Finish Clicked"  + gameState);
    }

    bool CheckIfTagExists(List<GameObject> allTroops, string tag)
    {
        int count = 0;
        foreach (GameObject troop in allTroops)
        {
            if (troop != null && troop.tag == tag)
            {
                count++;
            }
        }

        return count == 0;
    }

    void Initialize()
    {
        levelMessage.text = "";
        turnMessage.text = "";
        scoreMessage.text = "";
        actionDone = false;
        gameState = GameState.SHOP;
        cameraAudioSource = Camera.main.transform.Find("BackgroundMusic").GetComponent<AudioSource>();
    }


    
    public void PlayMusic(AudioClip clip)
    {
        cameraAudioSource.Stop();
        cameraAudioSource.clip = clip;
        cameraAudioSource.Play();
    }




}

