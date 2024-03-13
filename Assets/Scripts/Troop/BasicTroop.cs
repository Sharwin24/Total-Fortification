using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class BasicSoldier : TroopBase
{
    //The minimum armor for a heavy armor soldier
    public int HeavyArmorMinimum = 5;
    //Reference to the animator controller for the melee attack
    //Drag and Drop the controller from the project window
    public RuntimeAnimatorController meleeController;
    public RuntimeAnimatorController rangeController;

    private string[] TroopPrefabTags = { "HeavyArmorRange", "HeavyArmorMelee", "LightArmorRange", "LightArmorMelee" };
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public override void UpdateAppearance()
    {
        if (IsRange && Armor > HeavyArmorMinimum)
        {
            SetActiveByTag("HeavyArmorRange");
        }
        else if (IsRange && Armor <= HeavyArmorMinimum)
        {
            SetActiveByTag("LightArmorRange");
        }
        else if (!IsRange && Armor > HeavyArmorMinimum)
        {
            SetActiveByTag("HeavyArmorMelee");
        }
        else if (!IsRange && Armor <= HeavyArmorMinimum)
        {
            SetActiveByTag("LightArmorMelee");

        }
    }

    private void SetActiveByTag(string activeTag)
    {
        if (System.Array.IndexOf(TroopPrefabTags, activeTag) < 0)
        {
            Debug.LogError("SetActiveByTag called with unrecognized tag: " + activeTag);
            return;
        }
        foreach (Transform child in transform)
        {
            if (System.Array.Exists(TroopPrefabTags, tag => child.CompareTag(tag)))
            {
                child.gameObject.SetActive(child.CompareTag(activeTag));
            }
        }
    }

    public override void UpdateAnimation()
    {
          animator.runtimeAnimatorController = IsRange ? rangeController : meleeController;
    }
}
