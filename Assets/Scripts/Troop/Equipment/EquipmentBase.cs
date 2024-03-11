using UnityEngine;

public abstract class EquipmentBase : ScriptableObject, IEquipment
{
    // Use serialized fields for properties showing in the editor
    [SerializeField] private float healthModifier = 0;
    [SerializeField] private float armorModifier = 0;
    [SerializeField] private float attackPowerModifier = 0;
    [SerializeField] private float attackRangeModifier = 0;
    [SerializeField] private float moveRangeModifier = 0;
    [SerializeField] private float speedModifier = 0;
    [SerializeField] private Sprite equipmentIcon;
    [SerializeField] private EquipmentType equipmentType;

    [SerializeField] private string equipmentName = "";
    [SerializeField] private string equipmentDescription = "";

    public float HealthModifier => healthModifier;
    public float ArmorModifier => armorModifier;
    public float AttackPowerModifier => attackPowerModifier;
    public float AttackRangeModifier => attackRangeModifier;
    public float MoveRangeModifier => moveRangeModifier;
    public float SpeedModifier => speedModifier;
    public Sprite EquipmentIcon => equipmentIcon;
    public EquipmentType EquipmentType => equipmentType;
    public string EquipmentName => equipmentName;
    public string EquipmentDescription => equipmentDescription;

    public virtual bool IsWeapon { get; } = false;
    public virtual bool IsArmor { get; } = false;

}