using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;
    public ItemSO[] SOslots;

    private Image selector;
    public int lastEquippedSlot;
    private int lastSlot;
    public bool weaponEquipped;

    void Start ()
    {
        selector = slots[0].GetComponent<Image>();
        selector.color = new Color(selector.color.r, selector.color.g, selector.color.b, 0.4f);
        lastEquippedSlot = 34;
        lastSlot = 0;
        weaponEquipped = false;
    }

    public void UpdateSelectedSlot(int slot)
    {
        selector = slots[slot].GetComponent<Image>();
        selector.color = new Color(selector.color.r, selector.color.g, selector.color.b, 0.4f);
        selector = slots[lastSlot].GetComponent<Image>();
        selector.color = new Color(selector.color.r, selector.color.g, selector.color.b, 0f);
        lastSlot = slot;
    }

    public void EquipWeapon(int slot)
    {   
        //slots[lastEquippedSlot].GetComponent<SlotScript>().RemoveEquipMarker();
        SOslots[slot].Equip();
        slots[slot].GetComponent<SlotScript>().PlaceEquipMarker();
        weaponEquipped = true;
        lastEquippedSlot = slot;
    }

    public void SwitchWeapon(int slot){
        SOslots[lastEquippedSlot].Unequip();
        slots[lastEquippedSlot].GetComponent<SlotScript>().RemoveEquipMarker();
        SOslots[slot].Equip();
        slots[slot].GetComponent<SlotScript>().PlaceEquipMarker();
        //weaponEquipped = true;
        lastEquippedSlot = slot;
    }

    public void UnequipWeapon(int slot)
    {
        slots[slot].GetComponent<SlotScript>().RemoveEquipMarker();
        SOslots[slot].Unequip();
        weaponEquipped = false;
    }
}
