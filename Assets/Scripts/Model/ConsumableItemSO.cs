using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Items/Consumable")]

public class ConsumableItemSO : ItemSO
{
    [field: SerializeField]
    public int HealthRegen {get; set;} = 0;

    public override void Use()
    {
        base.Use();
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>().currentHealth += HealthRegen;
    }
}