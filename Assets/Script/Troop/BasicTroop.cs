using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSoldier : TroopBase
{

   
    public override void Attack(ITroop target)
    {
        Debug.Log("BasicSoldier attacking with power: " + AttackPower);
        // Implement attack logic, for example:
        target.TakeDamage(AttackPower, 0); 
        
    }

    public override void MoveTo(Vector3 position)
    {
        Debug.Log("BasicSoldier moving to position: " + position);
        // Implement movement logic, for example:
        transform.position = position;
    }
}
