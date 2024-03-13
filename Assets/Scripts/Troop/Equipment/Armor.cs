using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Equipment/Armor")]
public class Armor : EquipmentBase
{
      void OnEnable()
    {
        isArmor = true;
    }

}