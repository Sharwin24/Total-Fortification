using UnityEngine;

public interface IEquipment
{
    float HealthModifier { get; }
    float ArmorModifier { get; }
    float AttackPowerModifier { get; }
    float AttackRangeModifier { get; }
    float MoveRangeModifier { get; }
    float SpeedModifier { get; }

    bool IsWeapon { get;}
    bool IsArmor { get;}

    bool IsRangeWeapon { get;}

    Sprite EquipmentIcon { get; }
    EquipmentType EquipmentType { get; }
    string EquipmentName { get; }
    string EquipmentDescription { get; }

    // The level this equipment is unlocked at
    int EquipmentLevel { get; } 
}