using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentTest : MonoBehaviour
{
    public BasicSoldier troop; // Reference to the Troop object
    public EquipmentBase weaponToEquip; // The weapon to equip when the button is clicked

    // Call this method when the button is clicked
    public void EquipWeapon()
    {
       
        if(troop != null && weaponToEquip != null)
        {
            troop.EquipItem(weaponToEquip); 
            
        }
    }

    public void RemoveLeftArm()
    {
        if(troop != null)
        {
            troop.RemoveItem(EquipmentType.LeftArm);
        }
    }
}
