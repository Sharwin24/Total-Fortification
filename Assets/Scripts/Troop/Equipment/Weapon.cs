using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Equipment/Weapon")]
public class Weapon : EquipmentBase
{
    public override bool IsWeapon { get; } = true;


}