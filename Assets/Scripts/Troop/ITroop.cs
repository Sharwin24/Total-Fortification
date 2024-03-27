using System.Collections.Generic;
using System.Collections;
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
    bool IsRange { get; set; }

    Dictionary<EquipmentType, BodyPart> BodyParts { get; }

    IEnumerator Attack(ITroop target);

    IEnumerator MoveTo(Vector3 position);

    void TakeDamage(float damage);
    // Returns true if the item was equipped, false otherwise
    bool EquipItem(IEquipment equipment);
    // Returns true if the item was unEquipped, false otherwise
    bool RemoveItem(EquipmentType equipmentType);
    //Update the appearance of the troop given its armor and weapon
    void UpdateAppearance();
    //Update the animation controller of the troop given its ranged or not
    void UpdateAnimation();
    //Calculate the score of this troop, used for enemy
    int GetTroopScore();
}