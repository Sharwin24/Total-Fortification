using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class TroopBase : MonoBehaviour, ITroop {
    // Currently the weapons only increase the status, with no effect.
    [SerializeField] private float health;
    [SerializeField] private float armor;
    [SerializeField] private float moveRange;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackPower;
    [SerializeField] private bool isRange;

    // Public getters for encapsulation, private setters for controlled modification
    public float Health { get => health; set => health = value; }
    public float Armor { get => armor; set => armor = value; }
    public float MoveRange { get => moveRange; set => moveRange = value; }
    public float Speed { get => speed; set => speed = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public float AttackPower { get => attackPower; set => attackPower = value; }
    public bool IsRange { get => isRange; set => isRange = value; }
    public List<IEquipment> equippedItems = new();
    //The default equipments a troop has, used for enemy troops.
    public EquipmentBase[] defaultEquipments;
    public Dictionary<EquipmentType, BodyPart> BodyParts { get; } = new Dictionary<EquipmentType, BodyPart>();

    public HealthBar healthBar;

    protected Animator[] animators;

    public int AppearanceRange = 2;

    public int MoveSpeed = 2;

    public AudioClip rangedAttackSound;
    public AudioClip meleeAttackSound;

    public AudioClip deathSound;
    public AudioClip hitSound;

    public AudioClip moveSound;

    public Camera mainCamera;

    public int ScoreMultiplier = 1;

    public ScoreManager scoreManager;

    private AudioSource cameraAudioSource;
    private NavMeshAgent agent;

    protected virtual void Awake() {
        health = 100;
        armor = 0;
        moveRange = 7;
        speed = 1;
        attackRange = 5;
        attackPower = 10;
        isRange = false;

        //Increase the move and attack range by the appearance range
        //So that army won't collide with each other
        MoveRange += AppearanceRange;
        AttackRange += AppearanceRange;
        // Initialize each body part and add it to the dictionary
        BodyParts.Add(EquipmentType.Head, new BodyPart(EquipmentType.Head));
        BodyParts.Add(EquipmentType.Chest, new BodyPart(EquipmentType.Chest));
        BodyParts.Add(EquipmentType.LeftArm, new BodyPart(EquipmentType.LeftArm));
        BodyParts.Add(EquipmentType.RightArm, new BodyPart(EquipmentType.RightArm));
        BodyParts.Add(EquipmentType.Legs, new BodyPart(EquipmentType.Legs));

        healthBar = GetComponentInChildren<HealthBar>();
        animators = GetComponentsInChildren<Animator>(true);
        EquipItemInList();
        mainCamera = Camera.main;
        cameraAudioSource = mainCamera.GetComponent<AudioSource>();
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        cameraAudioSource.volume = volume;
        agent = GetComponent<NavMeshAgent>();

    }
    void Start() {
        scoreManager = ScoreManager.Instance;
        ScoreMultiplier = scoreManager.GetScoreMultiplier();
    }

    //Only called for enemy troops.
    private void EquipItemInList() {
        foreach (var equipment in defaultEquipments) {
            EquipItem(equipment);
        }
    }
    public virtual IEnumerator Attack(ITroop target) {
        if (target == null) {
            Debug.Log("No target to attack");
            yield return null;
        }
        if (Health > 0) {
            Vector3 position = ((TroopBase)target).transform.position;

            transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
            UpdateAnimationState(2, false);
            if (IsRange) {
                cameraAudioSource.clip = rangedAttackSound;
            } else {
                cameraAudioSource.clip = meleeAttackSound;
            }
            cameraAudioSource.Play();
            yield return new WaitForSeconds(2.5f);
            cameraAudioSource.Stop();
            target.TakeDamage(AttackPower);
            Debug.Log("Attacking");

            UpdateAnimationState(0);


        }

        OnActionComplete();

    }

    public virtual IEnumerator MoveTo(Vector3 position) {
        print("Moving to " + position);
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
        UpdateAnimationState(1, false); // Start walking/running animation
        cameraAudioSource.clip = moveSound;
        cameraAudioSource.Play();

        agent.SetDestination(position);

        float stationaryTime = 0f;

        while (Vector3.Distance(transform.position, position) > agent.stoppingDistance)
        {

            if (agent.velocity.sqrMagnitude < 0.03f)
            {
                stationaryTime += Time.deltaTime;
            }
            else
            {
                stationaryTime = 0f;
            }

            // Check if the agent has been stationary for more than 3 seconds
            if (stationaryTime >= 1f)
            {
                print("Agent has been stationary for too long - exiting loop");
                break;
            }

            yield return null;
        }

        print("reached destination - reset");
        agent.ResetPath();

        UpdateAnimationState(0);// Switch to idle animation
        cameraAudioSource.Stop();

        OnActionComplete();

        yield return new WaitForSeconds(0f);
    }

    public virtual void TakeDamage(float physicalDamage) {
        AudioSource.PlayClipAtPoint(hitSound, mainCamera.transform.position);
        transform.LookAt(transform);
        Health -= physicalDamage;
        healthBar.SetHealth(Health);

        if (Health <= 0) {
            LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();


            if (gameObject.tag == "Ally") {
                levelManager.allyCount--;
            } else if (gameObject.tag == "Enemy") {
                levelManager.enemyCount--;
                ScoreManager.Instance.AddScore(GetTroopScore());
            } else {
                throw new Exception("Invalid tag for troop");
            }

            UpdateAnimationState(3);
            AudioSource.PlayClipAtPoint(deathSound, mainCamera.transform.position);
            Destroy(gameObject, 5);
        }
    }
    private void UpdateAnimationState(int state = 0, bool applyRootMotion = false) {
        Debug.Log("UpdateAnimationState" + state);
        foreach (var childAnimator in animators) {
            childAnimator.SetInteger("animState", state);
            childAnimator.applyRootMotion = applyRootMotion;
        }
    }

    public bool HasEquipmentOfType(EquipmentType equipmentType, out IEquipment equipment) {
        foreach (IEquipment item in equippedItems) {
            if (item.EquipmentType == equipmentType) {
                equipment = item;
                return true;
            } else if (equipmentType == EquipmentType.TwoHanded && (item.EquipmentType == EquipmentType.LeftArm || item.EquipmentType == EquipmentType.RightArm)) {
                equipment = item;
                return true;
            } else if ((equipmentType == EquipmentType.LeftArm || equipmentType == EquipmentType.RightArm) && item.EquipmentType == EquipmentType.TwoHanded) {
                equipment = item;
                return true;
            }
        }
        equipment = null;
        return false;
    }

    public virtual bool EquipItem(IEquipment item)
    {

        bool equipped = false;
        if (item.EquipmentType == EquipmentType.TwoHanded) {
            RemoveItem(EquipmentType.LeftArm);
            RemoveItem(EquipmentType.RightArm);
            // RemoveItem(EquipmentType.TwoHanded);
            BodyParts[EquipmentType.LeftArm].equippedItem = item;
            BodyParts[EquipmentType.RightArm].equippedItem = item;
            //Why the following line of code will prevent the ApplyEquipmentModifiers
            //from adding the modifier to the current status? It is called with correct value, but the addition is not working.
            //BodyParts[EquipmentType.TwoHanded].equippedItem = item;

            equipped = true;
            Debug.Log("Equipped two handed weapon");
        }
        BodyParts.TryGetValue(item.EquipmentType, out BodyPart a);
        if (BodyParts.TryGetValue(item.EquipmentType, out BodyPart bodyPartToEquip)) {

            RemoveItem(item.EquipmentType);
            bodyPartToEquip.equippedItem = item;

            equipped = true;
        }
        if (equipped) {
            equippedItems.Add(item);
            ApplyEquipmentModifiers(item);
            Debug.Log("Equipped " + attackPower);
            IsRange = IsRange ? IsRange : item.IsRangeWeapon;
            UpdateAppearance();
            UpdateAnimation();
            return true;
        }
        return false;
    }

    public virtual bool RemoveItem(EquipmentType equipmentType) {
        bool removed = false;
        if (equipmentType == EquipmentType.TwoHanded)
        {
            RemoveEquipmentModifiers(BodyParts[EquipmentType.LeftArm].equippedItem);
            equippedItems.Remove(BodyParts[EquipmentType.LeftArm].equippedItem);
            equippedItems.Remove(BodyParts[EquipmentType.RightArm].equippedItem);
            BodyParts[EquipmentType.LeftArm].equippedItem = null;
            BodyParts[EquipmentType.RightArm].equippedItem = null;
            // BodyParts[EquipmentType.TwoHanded].equippedItem = null;
            removed = true;
        }
        BodyPart bodyPartToUnEquip = FindBodyPart(equipmentType);
        if (bodyPartToUnEquip != null && bodyPartToUnEquip.equippedItem != null) {
            RemoveEquipmentModifiers(bodyPartToUnEquip.equippedItem);
            if (bodyPartToUnEquip.equippedItem.IsRangeWeapon) {
                IsRange = false;
            }
            if (bodyPartToUnEquip.equipmentType == EquipmentType.TwoHanded) {
                equippedItems.Remove(bodyPartToUnEquip.equippedItem);
                BodyParts[EquipmentType.LeftArm].equippedItem = null;
                BodyParts[EquipmentType.RightArm].equippedItem = null;
                BodyParts[EquipmentType.TwoHanded].equippedItem = null;
                removed = true;
            } else
            {
                equippedItems.Remove(bodyPartToUnEquip.equippedItem);
                bodyPartToUnEquip.equippedItem = null;
                removed = true;
            }

        }

        if (removed) {
            UpdateAnimation();
            UpdateAppearance();
            return true;
        }

        return false;
    }

    // Helper method to find the body part object by type
    protected BodyPart FindBodyPart(EquipmentType type) {
        BodyPart bodyPart;
        if (BodyParts.TryGetValue(type, out bodyPart)) {
            return bodyPart;
        }
        return null; // No matching body part found
    }

    private void ApplyEquipmentModifiers(IEquipment item) {
        Debug.Log(item.EquipmentType);
        health += item.HealthModifier;
        armor += item.ArmorModifier;
        attackPower += item.AttackPowerModifier;
        attackRange += item.AttackRangeModifier;
        moveRange += item.MoveRangeModifier;
        speed += item.SpeedModifier;
        Debug.Log("Applied " + item.AttackPowerModifier);
        Debug.Log("After Applied " + AttackPower);
    }

    private void RemoveEquipmentModifiers(IEquipment item) {
        health -= item.HealthModifier;
        armor -= item.ArmorModifier;
        attackPower -= item.AttackPowerModifier;
        attackRange -= item.AttackRangeModifier;
        moveRange -= item.MoveRangeModifier;
        speed -= item.SpeedModifier;
    }

    public abstract void UpdateAppearance();

    public abstract void UpdateAnimation();

    public int GetTroopScore() {
        return (int)Math.Floor((Armor + AttackPower + AttackRange + MoveRange + Speed) * ScoreMultiplier);
    }

    private void OnActionComplete()
    {
        if (gameObject.tag == "Ally")
        {
            PlayerBehavior.actionTaken = true;
        }
        else if (gameObject.tag == "Enemy")
        {
            EnemyBehavior.actionTaken = true;
        }
        else
        {
            throw new InvalidOperationException("Invalid tag for troop");
        }
    }
}