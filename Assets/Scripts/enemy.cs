using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    public Animator animator;
    public KnockbackScript knockbackScript;
    private Rigidbody2D m_Rigidbody2D;

    public int maxHealth = 20;
    int currentHealth;
    private float knockbackStr = 80;




    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, Vector3 attacker)
    {
        currentHealth -= damage;
        Debug.Log("Character dealt " + damage + " damage");
        
        //Play hurt animation

        if(currentHealth <= 0)
        {
            //Die();
        }

        
        knockbackScript.knockback(attacker.x, knockbackStr);
    }

    void Die()
    {
        Debug.Log("Enemy died");

        animator.SetBool("isDead", true);

        int LayerTransparentFX = LayerMask.NameToLayer("TransparentFX");
        gameObject.layer = LayerTransparentFX;
        GetComponent<EnemyAI>().enabled = false;
        this.enabled = false;
    }

    //Update is called once per frame
    void FixedUpdate()
    {
        animator.SetFloat("Speed", Mathf.Abs(m_Rigidbody2D.velocity.x));
    }
}
