using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TroopBase : MonoBehaviour, ITroop
{
    // Implementing the ITroop interface
    public virtual int Health { get; set; }

    // Assuming speed doesn't change, hence no setter.
    public float speed => 5.0f;

    public int Move => 3;

    [SerializeField]
    private bool _isEnemy;

    public virtual bool isEnemy => _isEnemy;

    public bool IsRange => false; // Example: This could be a melee troop

    public int AttackRange { get; set; } = 3; // Melee default

    public int AttackPower { get; set; } = 10; // Example attack power

    //The health bar of the troop(in prefab canvas)
     public HealthBar healthBar;

    public virtual void Attack(ITroop target)
    {
        target.TakeDamage(AttackPower); 
    }

    public virtual void MoveTo(Vector3 position)
    {

        transform.position = position;
    }

    public virtual void TakeDamage(int physicalDamage)
    {
        Health -= physicalDamage;
        healthBar.SetHealth(Health);

        if (Health <= 0)
        {
            if (isEnemy) LevelManager.enemyCount--;
            
            Destroy(gameObject);
        }
    }

    // Additional methods for the armory system could be added here
}