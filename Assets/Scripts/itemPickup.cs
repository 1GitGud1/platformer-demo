using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{

    private InventoryScript inventory;
    public ItemSO itemSO;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScript>();
    }

    // Update is called once per frame
    public void PickUpItem()
    {
        for (int i = 0; i < inventory.slots.Length; i++){
            if (inventory.isFull[i] == false){
                //item added to inventory
                inventory.isFull[i] = true;
                inventory.SOslots[i] = itemSO;
                //game object with a sprite of the item is instantiated as a child of a slot
                Instantiate(itemSO.ItemSpriteObject, inventory.slots[i].transform, false);
                Destroy(gameObject);
                break;
            }
        }
    }
}
