using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{

    // MUST update LevelManager.actionDone upon ending execution!
    // cannot be static, will fix later.
   public static void TakeAction(GameObject currentTroop)
   {
        // Animator anim = currentTroop.GetComponent<Animator>();
        
        // Move once + (maybe) attack
        // Attack twice

        // Mark action as done
        LevelManager.actionDone = true;
    }
}
