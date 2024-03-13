using System;
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

    private List<string> TroopPrefabTags = new List<string> { "HeavyArmorRange", "HeavyArmorMelee", "LightArmorRange", "LightArmorMelee" };

    private List<string> TroopEquipmentAppearanceTags = new List<string> { "Sword", "Shield", "Bow" };
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    public override void UpdateAppearance()
    {
        if (IsRange && Armor > HeavyArmorMinimum)
        {
            SetActiveByTags(new List<string> { "HeavyArmorRange"}, TroopPrefabTags);
        }
        else if (IsRange && Armor <= HeavyArmorMinimum)
        {
            SetActiveByTags(new List<string> { "LightArmorRange" }, TroopPrefabTags);

        }
        else if (!IsRange && Armor > HeavyArmorMinimum)
        {
            SetActiveByTags(new List<string> { "HeavyArmorMelee" }, TroopPrefabTags);

        }
        else if (!IsRange && Armor <= HeavyArmorMinimum)
        {
            SetActiveByTags(new List<string> { "LightArmorMelee" }, TroopPrefabTags);

        }
    }

    private void SetActiveByTags(List<string> activeTags, List<string> totalTags)
    {
        foreach (var activeTag in activeTags)
        {
            if (!totalTags.Contains(activeTag))
            {
                Debug.LogError("SetActiveByTags called with unrecognized tag: " + activeTag);
                return;
            }
        }

        // Iterate through all children of this GameObject
        foreach (Transform child in transform)
        {
            // Check if the child has one of the total tags
            bool hasTotalTag = false;
            foreach (var totalTag in totalTags)
            {
                if (child.CompareTag(totalTag))
                {
                    hasTotalTag = true;
                    break;
                }
            }

            if (hasTotalTag)
            {
                // Activate the child if it has any of the active tags, otherwise deactivate it
                bool shouldBeActive = false;
                foreach (var activeTag in activeTags)
                {
                    if (child.CompareTag(activeTag))
                    {
                        shouldBeActive = true;
                        break;
                    }
                }
                child.gameObject.SetActive(shouldBeActive);
            }
        }
    }

    public override void UpdateAnimation()
    {
          animator.runtimeAnimatorController = IsRange ? rangeController : meleeController;
    }
}
