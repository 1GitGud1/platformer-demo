using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]

public class ItemSO : ScriptableObject
{
    [field: SerializeField]
    public bool IsStackable {get; set;}

    public int ID => GetInstanceID();

    [field: SerializeField]
    public int MaxStackSize {get; set;} = 1;

    [field: SerializeField]
    public string Name {get; set;}

    [field: TextArea]
    [field: SerializeField]
    public string Description {get; set;}

    [field: SerializeField]
    public Sprite ItemSprite {get; set;}

    [field: SerializeField]
    public GameObject ItemSpriteObject {get; set;}

    [field: SerializeField]
    public GameObject item;

    [field: SerializeField]
    public ItemType itemType; // An enum that defines the item's type

    public virtual void Use()
    {
        // This method can be overridden by child classes to define item-specific behavior
        Debug.Log("Using item: " + Name);
    }

    public virtual void Equip()
    {
        Debug.Log("Equiping item: " + Name);
    }

    public virtual void Unequip()
    {
        Debug.Log("Unequiping item: " + Name);
    }

    public void DropItem(Vector2 playerPosition)
    {
        //instantiates the specified item game object at the player position
        Instantiate(item, playerPosition, Quaternion.identity);
    }
}

public enum ItemType
{
    Weapon,
    Armor,
    Accessory,
    Consumable,
    // Add more types as needed
}
