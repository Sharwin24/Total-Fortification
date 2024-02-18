using UnityEngine;

public interface ITroop
{
    //The amount of health the troop has
    int Health { get; set; }
    //The speed at which the troop has for turn
    float speed { get; }
    //The number of tiles the troop can move in one turn
    int Move {  get; }

    //Is the troop a ranged troop(arrow, magic, etc)
    bool IsRange { get; }
    //The attack range of the troop,(melee should be 1)
    int AttackRange { get; set;}

    //The attack power of the troop(physical damage)
    int AttackPower { get; set;}
    void Attack(ITroop target);

    void MoveTo(Vector3 position);
    void TakeDamage(int physicalDamage, int magicalDamage);

    //We also needs the armory system below
}