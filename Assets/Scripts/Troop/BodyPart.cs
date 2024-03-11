[System.Serializable]
public class BodyPart
{
    public EquipmentType equipmentType;
    public IEquipment equippedItem;

    // Constructor to specify the equipment type for clarity and safety
    public BodyPart(EquipmentType type)
    {
        equipmentType = type;
    }


    public void EquipItem(IEquipment item)
    {
        equippedItem = item;
    }
}