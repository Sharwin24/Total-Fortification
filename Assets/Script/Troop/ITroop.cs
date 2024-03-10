using UnityEngine;

public interface ITroop
{
    //The amount of health the troop has
    float Health { get; set; }
    float Armor { get; set; }

    //The attack range of the troop,(melee should be 1)
    float AttackRange { get; set; }

    //The attack power of the troop(physical damage)
    float AttackPower { get; set; }
    float MoveRange { get; set; }

    float Speed { get; set; }

    //Is the troop a ranged troop(arrow, magic, etc)
    bool IsRange { get; set;}
    void Attack(ITroop target);

    void MoveTo(Vector3 position);
    void TakeDamage(float damage);
}