using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public abstract class TroopBase : MonoBehaviour, ITroop
{
    // Currently the weapons only increase the status, with no effect.
    [SerializeField] private float health = 100.0f;
    [SerializeField] private float armor = 0.0f;
    [SerializeField] private float moveRange = 5;
    [SerializeField] private float speed = 1;
    [SerializeField] private float attackRange = 3;
    [SerializeField] private float attackPower = 10;
    [SerializeField] private bool isRange = false;

    // Public getters for encapsulation, private setters for controlled modification
    public float Health { get => health; set => health = value; }
    public float Armor { get => armor; set => armor = value; }
    public float MoveRange { get => moveRange; set => moveRange = value; }
    public float Speed { get => speed; set => speed = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public float AttackPower { get => attackPower; set => attackPower = value; }
    public bool IsRange { get => isRange; set => isRange = value; }

    public Dictionary<EquipmentType, BodyPart> BodyParts { get; } = new Dictionary<EquipmentType, BodyPart>();

    public HealthBar healthBar;

    void Awake()
    {
        // Initialize each body part and add it to the dictionary
        BodyParts.Add(EquipmentType.Head, new BodyPart(EquipmentType.Head));
        BodyParts.Add(EquipmentType.Chest, new BodyPart(EquipmentType.Chest));
        BodyParts.Add(EquipmentType.LeftArm, new BodyPart(EquipmentType.LeftArm));
        BodyParts.Add(EquipmentType.RightArm, new BodyPart(EquipmentType.RightArm));
        BodyParts.Add(EquipmentType.Legs, new BodyPart(EquipmentType.Legs));

        healthBar = GetComponentInChildren<HealthBar>();
    }

    //The health bar of the troop(in prefab canvas)


    public virtual void Attack(ITroop target)
    {
        target.TakeDamage(AttackPower);
    }

    public virtual void MoveTo(Vector3 position)
    {

        transform.position = position;
    }

    public virtual void TakeDamage(float physicalDamage)
    {
        Health -= physicalDamage;
        healthBar.SetHealth(Health);

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }



    public virtual bool EquipItem(IEquipment item)
    {
        if (item.EquipmentType == EquipmentType.TwoHanded)
        {
            RemoveItem(EquipmentType.LeftArm);
            RemoveItem(EquipmentType.RightArm);
            BodyParts[EquipmentType.LeftArm].equippedItem = item;
            BodyParts[EquipmentType.RightArm].equippedItem = item;
            ApplyEquipmentModifiers(item);
            return true;
        }
        Debug.Log(BodyParts[EquipmentType.LeftArm].equipmentType);
        if (BodyParts.TryGetValue(item.EquipmentType, out BodyPart bodyPartToEquip) && bodyPartToEquip.equipmentType == item.EquipmentType)
        {
            Debug.Log("Equipping " + item.EquipmentType);
            RemoveItem(item.EquipmentType);
            bodyPartToEquip.equippedItem = item;
            ApplyEquipmentModifiers(item);
            return true;
        }
        return false;
    }

    public virtual bool RemoveItem(EquipmentType equipmentType)
    {
        BodyPart bodyPartToUnEquip = FindBodyPart(equipmentType);
        if (bodyPartToUnEquip != null && bodyPartToUnEquip.equippedItem != null)
        {
            if (bodyPartToUnEquip.equipmentType == EquipmentType.TwoHanded)
            {
                RemoveEquipmentModifiers(bodyPartToUnEquip.equippedItem);
                BodyParts[EquipmentType.LeftArm].equippedItem = null;
                BodyParts[EquipmentType.RightArm].equippedItem = null;
                return true;
            }
            else
            {
                RemoveEquipmentModifiers(bodyPartToUnEquip.equippedItem);
                bodyPartToUnEquip.equippedItem = null;
                return true;
            }

        }

        return false;
    }

    // Helper method to find the body part object by type
    private BodyPart FindBodyPart(EquipmentType type)
    {
        BodyPart bodyPart;
        if (BodyParts.TryGetValue(type, out bodyPart))
        {
            return bodyPart;
        }
        return null; // No matching body part found
    }

    private void ApplyEquipmentModifiers(IEquipment item)
    {
        Health += item.HealthModifier;
        Armor += item.ArmorModifier;
        AttackPower += item.AttackPowerModifier;
        AttackRange += item.AttackRangeModifier;
        MoveRange += item.MoveRangeModifier;
        Speed += item.SpeedModifier;
    }

    private void RemoveEquipmentModifiers(IEquipment item)
    {
        Health -= item.HealthModifier;
        Armor -= item.ArmorModifier;
        AttackPower -= item.AttackPowerModifier;
        AttackRange -= item.AttackRangeModifier;
        MoveRange -= item.MoveRangeModifier;
        Speed -= item.SpeedModifier;
    }

    public abstract void UpdateAppearance();

    public abstract void UpdateAnimation();


}