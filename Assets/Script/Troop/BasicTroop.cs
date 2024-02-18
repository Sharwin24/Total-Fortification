using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSoldier : TroopBase
{
    public override void Attack(ISoldier target)
    {
        Debug.Log("BasicSoldier attacking with power: " + AttackPower);
        // Implement attack logic, for example:
        target.TakeDamage(attackPower, 0); // Assuming physical damage only for simplicity
    }

    // Optionally override other methods if specific behavior is needed
    protected override void Die()
    {
        base.Die();
        // Additional behavior specific to BasicSoldier's death, if necessary
    }
}
