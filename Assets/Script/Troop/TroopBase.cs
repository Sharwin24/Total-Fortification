using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TroopBase : MonoBehaviour, ITroop
{
    // Implementing the ITroop interface
    public int Health { get; set; }

    // Assuming speed doesn't change, hence no setter.
    public float speed => 5.0f;

    public int Move => 3;

    public bool IsRange => false; // Example: This could be a melee troop

    public int AttackRange { get; set; } = 1; // Melee default

    public int AttackPower { get; set; } = 10; // Example attack power

    public virtual void Attack(ITroop target)
    {
        target.TakeDamage(AttackPower, 0); 
    }

    public virtual void MoveTo(Vector3 position)
    {

        transform.position = position;
    }

    public virtual void TakeDamage(int physicalDamage, int magicalDamage)
    {
        Health -= physicalDamage + magicalDamage;

        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    // Additional methods for the armory system could be added here
}