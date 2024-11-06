using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]

public class WeaponItemSO : ItemSO
{
    [field: SerializeField]
    public int DamageBonus {get; set;} = 0;

    [field: SerializeField]
    public float RangeBonus {get; set;} = 0;

    [field: SerializeField]
    public float AttackSpeedBonus {get; set;} = .5f; //todo: review the attack script, possibly redesign how attack speed works (instead make an animation event which lets you attack after the initial attack animation finishes)

    [field: SerializeField]
    public WeaponType weaponType;

    public override void Equip()
    {
        base.Equip();
        GameObject.FindGameObjectWithTag("Player").GetComponent<AttackScript>().attackDamage += DamageBonus;
        GameObject.FindGameObjectWithTag("Player").GetComponent<AttackScript>().attackRange += RangeBonus;
        GameObject.FindGameObjectWithTag("Player").GetComponent<AttackScript>().attackRate += AttackSpeedBonus;
    }

    public override void Unequip()
    {
        base.Unequip();
        GameObject.FindGameObjectWithTag("Player").GetComponent<AttackScript>().attackDamage -= DamageBonus;
        GameObject.FindGameObjectWithTag("Player").GetComponent<AttackScript>().attackRange -= RangeBonus;
        GameObject.FindGameObjectWithTag("Player").GetComponent<AttackScript>().attackRate -= AttackSpeedBonus;
    }
}

public enum WeaponType
{
    Stab,
    Swing,
    Bow,
    Throw,
    // Add more types as needed
}

