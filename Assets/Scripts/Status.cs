using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class Status : MonoBehaviour
{
    public WeaponInventory inventory;
    
    public Image slot, slot0, slot1, slot2;
    public Sprite hand;

    
    private void OnEnable()
    {

        Debug.Log("Actualizar Slots");

        if (SaveManager.Instance.currentData.takenCollectables.Contains("Hand"))
        {
            slot.sprite = hand;
        }

        if (inventory.equippedWeapons[0] != null)
        {
            var Weapon = inventory.equippedWeapons[0].GetComponent<Weapon>();
            slot0.sprite = Weapon.icon;
        }

        if (inventory.equippedWeapons[1] != null)
        {
            var Weapon = inventory.equippedWeapons[1].GetComponent<Weapon>();
            slot1.sprite = Weapon.icon;
        }

        if (inventory.equippedWeapons[2] != null)
        {
            var Weapon = inventory.equippedWeapons[2].GetComponent<Weapon>();
            slot2.sprite = Weapon.icon;
        }
    }
}
