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
    //The default equipments a troop has, used for enemy troops.
    public EquipmentBase[] equipments;
    public Dictionary<EquipmentType, BodyPart> BodyParts { get; } = new Dictionary<EquipmentType, BodyPart>();

    public HealthBar healthBar;

    protected Animator[] animators;

    public int AppearanceRange = 2;
    public float MinimumContactDistance;

    protected virtual void Awake()
    {
        //Increase the move and attack range by the appearance range
        //So that army won't collide with each other
        // MoveRange += AppearanceRange;
        // AttackRange += AppearanceRange;
        // MinimumContactDistance = AppearanceRange + 1;
        // Initialize each body part and add it to the dictionary
        BodyParts.Add(EquipmentType.Head, new BodyPart(EquipmentType.Head));
        BodyParts.Add(EquipmentType.Chest, new BodyPart(EquipmentType.Chest));
        BodyParts.Add(EquipmentType.LeftArm, new BodyPart(EquipmentType.LeftArm));
        BodyParts.Add(EquipmentType.RightArm, new BodyPart(EquipmentType.RightArm));
        BodyParts.Add(EquipmentType.Legs, new BodyPart(EquipmentType.Legs));

        healthBar = GetComponentInChildren<HealthBar>();
        animators = GetComponentsInChildren<Animator>(true);
        EquipItemInList();
        
    }
    void Start()
    {
       
    }

    //Only called for enemy troops.
    private void EquipItemInList()
    {
        foreach (var equipment in equipments)
        {
            EquipItem(equipment);
        }
    }
    public virtual IEnumerator Attack(ITroop target)
    {
        if (target == null)
        {
            Debug.Log("No target to attack");
            yield return null;
        }
        // Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 position = ((TroopBase)target).transform.position;
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));

        UpdateAnimationState(2, false);
        target.TakeDamage(AttackPower);
        yield return new WaitForSeconds(2f);
        Debug.Log("Attacking");

        UpdateAnimationState(0);
    }

    public virtual IEnumerator MoveTo(Vector3 position)
    {
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
        UpdateAnimationState(1,false); // Start walking/running animation

        while (Vector3.Distance(transform.position, position) > 0.35f)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, Speed * Time.deltaTime);
            // yield return null;
             yield return new WaitForSeconds(0f);
        }

        UpdateAnimationState(0);// Switch to idle animation

        yield return new WaitForSeconds(0f);
    }

    public virtual void TakeDamage(float physicalDamage)
    {
        transform.LookAt(transform);
        Health -= physicalDamage;
        healthBar.SetHealth(Health);

        if (Health <= 0)
        {
            UpdateAnimationState(3);
            Destroy(gameObject, 3f);
        }
    }
    private void UpdateAnimationState(int state = 0, bool applyRootMotion = true)
    {
        Debug.Log("UpdateAnimationState" + state);
        foreach (var childAnimator in animators)
        {
            childAnimator.SetInteger("animState", state);
            childAnimator.applyRootMotion = applyRootMotion;
        }
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