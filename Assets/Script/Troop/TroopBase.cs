using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class TroopBase : MonoBehaviour, ISoldier
{
    protected int health = 100;
    protected int attackPower = 10;

    public int Health
    {
        get => health;
        set => health = value;
    }

    public int AttackPower => attackPower;

    public abstract void Attack(ISoldier target);

    public void TakeDamage(int physicalDamage, int magicalDamage)
    {
        // Example damage calculation, could be more sophisticated
        int totalDamage = physicalDamage + magicalDamage; // Simplified for demonstration
        health -= totalDamage;
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        Debug.Log("Soldier died.");
        // Additional death logic here, like playing animations or removing the soldier from the game.
    }
}