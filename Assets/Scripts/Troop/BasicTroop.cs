using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BasicSoldier : TroopBase
{
    //The minimum armor for a heavy armor soldier
    public int HeavyArmorMinimum = 5;
    //Reference to the animator controller for the melee attack
    //Drag and Drop the controller from the project window
    public RuntimeAnimatorController meleeController;
    public RuntimeAnimatorController rangeController;
    public RigBuilder[] rigBuilders;

    


    private bool hasShield = false;
    private bool hasSword = false;

    private List<string> TroopPrefabTags = new() { "HeavyArmorRange", "HeavyArmorMelee", "LightArmorRange", "LightArmorMelee" };

    private List<string> TroopEquipmentAppearanceTags = new() { "RightHandSword", "LeftHandShield", "TwoHandsBow" };
    private List<string> TroopAppearanceTags;


    protected override void Awake()
    {

        TroopAppearanceTags = new List<string>();
        TroopAppearanceTags.AddRange(TroopPrefabTags);
        TroopAppearanceTags.AddRange(TroopEquipmentAppearanceTags);
        rigBuilders = GetComponentsInChildren<RigBuilder>(true);


        base.Awake();
    }

    public override bool EquipItem(IEquipment item)
    {
        //show shield if the troop has some armor
        hasShield = item.IsArmor;
        hasSword = item.IsWeapon;

        return base.EquipItem(item);
    }

    public override bool RemoveItem(EquipmentType equipmentType)
    {
        var equipment = FindBodyPart(equipmentType).equippedItem;
        if (equipment != null)
        {
            hasShield = equipment.IsArmor ? false : hasShield;
            hasSword = equipment.IsWeapon ? false : hasSword;
        }
        return base.RemoveItem(equipmentType);
    }


    public override void UpdateAppearance()
    {

        List<string> tagsToActive = new();
        if (IsRange)
        {
            tagsToActive.Add("TwoHandsBow");
            tagsToActive.Add(Armor > HeavyArmorMinimum ? "HeavyArmorRange" : "LightArmorRange");
        }
        else
        {

            if (hasShield)
            {
                tagsToActive.Add("LeftHandShield");
            }
            if (hasSword)
            {
                tagsToActive.Add("RightHandSword");
            }
            tagsToActive.Add(Armor > HeavyArmorMinimum ? "HeavyArmorMelee" : "LightArmorMelee");
        }
        SetActiveByTags(tagsToActive, TroopAppearanceTags);
    }

    //For the given list of tags, activate the game objects with those tags and deactivate the rest
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
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
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
        UpdateChildAnimators();
        UpdateRigLayers(IsRange ? "RangeRigLayer" : "MeleeRigLayer");
    }

    private void UpdateChildAnimators()
    {
        Animator[] childAnimators = GetComponentsInChildren<Animator>(true);
        foreach (Animator childAnimator in childAnimators)
        {
            childAnimator.runtimeAnimatorController = IsRange ? rangeController : meleeController;
        }
        animators = childAnimators;
    }

    private void UpdateRigLayers(string activeLayerName)
    {
        foreach (RigBuilder rigBuilder in rigBuilders)
        {
            bool found = false;
            foreach (RigLayer layer in rigBuilder.layers)
            {
                bool isActive = layer.name == activeLayerName;
                layer.active = isActive;
                if (isActive) found = true;
            }
            if (found) rigBuilder.Build();
        }
    }


}
