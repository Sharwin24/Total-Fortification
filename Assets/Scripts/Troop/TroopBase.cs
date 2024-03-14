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
    [SerializeField] private float moveRange = 7;
    [SerializeField] private float speed = 1;
    [SerializeField] private float attackRange = 5;
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

    public EquipmentBase[] equipments;
    public Dictionary<EquipmentType, BodyPart> BodyParts { get; } = new Dictionary<EquipmentType, BodyPart>();

    public HealthBar healthBar;

    protected Animator animator;

    protected virtual void Awake()
    {
        // Initialize each body part and add it to the dictionary
        BodyParts.Add(EquipmentType.Head, new BodyPart(EquipmentType.Head));
        BodyParts.Add(EquipmentType.Chest, new BodyPart(EquipmentType.Chest));
        BodyParts.Add(EquipmentType.LeftArm, new BodyPart(EquipmentType.LeftArm));
        BodyParts.Add(EquipmentType.RightArm, new BodyPart(EquipmentType.RightArm));
        BodyParts.Add(EquipmentType.Legs, new BodyPart(EquipmentType.Legs));

        healthBar = GetComponentInChildren<HealthBar>();
        animator = GetComponent<Animator>();
        EquipItemInList();
    }

    //Only called for enemy troops.
    private void EquipItemInList(){
        foreach (var equipment in equipments)
        {
            EquipItem(equipment);
        }
    }
    public virtual void Attack(ITroop target)
    {
        // Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.LookAt(((TroopBase) target).transform);
        animator.applyRootMotion = false;
        animator.SetInteger("animState", 2);
        target.TakeDamage(AttackPower);
        // transform.position = position;
        Invoke(nameof(SetAnimationIdle), 2.5f);
    }

    public virtual IEnumerator MoveTo(Vector3 position)
    {
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
        animator.SetInteger("animState", 1); // Start walking/running animation

        while (Vector3.Distance(transform.position, position) > 0.35) {
            transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
            // If using root motion, the actual movement might be driven by the animation.
            // Otherwise, include code here to move towards the target position.
            yield return null;
        }

        animator.SetInteger("animState", 0); // Switch to idle animation
        
    }

    public virtual void TakeDamage(float physicalDamage)
    {
        transform.LookAt(transform);
        Health -= physicalDamage;
        healthBar.SetHealth(Health);

        if (Health <= 0)
        {
            animator.SetInteger("animState", 3);
            Destroy(gameObject, 3f);
        }
    }

    private void SetAnimationIdle()
    {
        animator.SetInteger("animState", 0);
        animator.applyRootMotion = true;
    }



    public virtual bool EquipItem(IEquipment item)
    {
        bool equipped = false;
        if (item.EquipmentType == EquipmentType.TwoHanded)
        {
            RemoveItem(EquipmentType.LeftArm);
            RemoveItem(EquipmentType.RightArm);
            BodyParts[EquipmentType.LeftArm].equippedItem = item;
            BodyParts[EquipmentType.RightArm].equippedItem = item;

            equipped = true;
        }
        BodyParts.TryGetValue(item.EquipmentType, out BodyPart a);
        if (BodyParts.TryGetValue(item.EquipmentType, out BodyPart bodyPartToEquip))
        {

            RemoveItem(item.EquipmentType);
            bodyPartToEquip.equippedItem = item;

            equipped = true;
        }
        if (equipped)
        {
            ApplyEquipmentModifiers(item);
            IsRange = item.IsRangeWeapon;
            UpdateAppearance();
            UpdateAnimation();
            return true;
        }
        return false;
    }

    public virtual bool RemoveItem(EquipmentType equipmentType)
    {
        bool removed = false;
        BodyPart bodyPartToUnEquip = FindBodyPart(equipmentType);
        if (bodyPartToUnEquip != null && bodyPartToUnEquip.equippedItem != null)
        {
            RemoveEquipmentModifiers(bodyPartToUnEquip.equippedItem);
            if (bodyPartToUnEquip.equippedItem.IsRangeWeapon)
            {
                IsRange = false;
            }
            if (bodyPartToUnEquip.equipmentType == EquipmentType.TwoHanded)
            {
                BodyParts[EquipmentType.LeftArm].equippedItem = null;
                BodyParts[EquipmentType.RightArm].equippedItem = null;
                removed = true;
            }
            else
            {
                bodyPartToUnEquip.equippedItem = null;
                removed = true;
            }

        }

        if (removed)
        {
            UpdateAnimation();
            UpdateAppearance();
            return true;
        }

        return false;
    }

    // Helper method to find the body part object by type
    protected BodyPart FindBodyPart(EquipmentType type)
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