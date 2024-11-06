using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Animator animator;
    public KnockbackScript knockbackScript;
    public CharacterController2D controller;
    public HUDScript HUD;
    public Collider2D coll;
    private PhysicsMaterial2D deadMaterial;
    

    public int maxHealth = 100;
    public int currentHealth;
    float HUDHealth;
    float HUDdamage;
    public float knockbackStr = 80;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        HUDHealth = maxHealth;
        HUD.setMaxHealth(maxHealth);
        deadMaterial = new PhysicsMaterial2D(coll.sharedMaterial.name + " (Instance)");
        deadMaterial.friction = 0.4f;
    }

    public void TakeDamage(int damage, Vector3 attacker)
    {
        currentHealth -= damage;
        HUDdamage = (float)damage;
        Debug.Log("Player took " + damage + " damage");
        
        //Play hurt animation

        if(currentHealth <= 0)
        {
            Die();
        }

        controller.m_AirControl = false;
        knockbackScript.knockback(attacker.x, knockbackStr);
    }

    void Die()
    {
        Debug.Log("YOU DIED");

        animator.SetBool("isDead", true);

        int LayerTransparentFX = LayerMask.NameToLayer("TransparentFX");
        gameObject.layer = LayerTransparentFX;
        coll.sharedMaterial = deadMaterial;

        GetComponent<CharacterMovement>().enabled = false;
        controller.enabled = false;
        //this.enabled = false;
    }
    
    void Update()
    {
        HUDHealth = Mathf.Lerp(HUDHealth, currentHealth, Time.deltaTime * 10);
        HUD.setHealth(HUDHealth);
    }
}
