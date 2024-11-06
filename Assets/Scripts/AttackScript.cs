using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public Transform attackCheck;

    public int attackDamage = 5;
    public float attackRange = 0.5f;
    public float attackRate = 1f;
    public LayerMask enemyLayers;

    public void Attack()
    {
        //detect and store enemies in range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCheck.position, attackRange, enemyLayers);

        //deal damage
        foreach(Collider2D Enemy in hitEnemies)
        {
            IDamageable damageable = Enemy.GetComponent<IDamageable>();
            if (damageable != null){
                damageable.TakeDamage(attackDamage, attackCheck.position);
            }
        }
    }

    public struct AttackInfo
    {
        public int DMG;
        public Vector3 point;

        public AttackInfo(int _DMG, Vector3 _point)
        {
            DMG = _DMG;
            point = _point;
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackCheck == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(attackCheck.position, attackRange);
    }
}
