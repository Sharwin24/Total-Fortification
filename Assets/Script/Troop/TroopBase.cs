using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TroopBase : MonoBehaviour, ITroop
{
    // Implementing the ITroop interface
    public float Health { get; set; } = 100.0f;

    public float Armor { get; set; } = 0.0f;

    public float MoveRange { get; set; } = 3;

    public float Speed { get; set; } = 1;

    public float AttackRange { get; set; } = 1;

    public float AttackPower { get; set; } = 10;

    public bool IsRange { get; set;} = false;
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

    public virtual void TakeDamage(float physicalDamage)
    {
        Health -= physicalDamage;
        healthBar.SetHealth(Health);

        if (Health <= 0)
        {
            
            Destroy(gameObject);
        }
    }
}