using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    public static int enemyCount = 0;
    public TextMeshProUGUI levelStatus;

    void Start()
    {
        levelStatus.text = "";
    }


    void Update()
    {   
        Debug.Log(enemyCount);
        if (enemyCount == 0) {
            levelStatus.text = "Level Beat!";
        }
    }
}
