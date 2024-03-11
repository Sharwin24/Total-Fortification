using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Equipment/Armor")]
public class Armor : EquipmentBase
{
  public override bool IsArmor { get; } = true;

}