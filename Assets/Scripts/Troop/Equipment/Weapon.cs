using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Equipment/Weapon")]
public class Weapon : EquipmentBase
{
    void OnEnable()
    {
        isWeapon = true;
    }
}