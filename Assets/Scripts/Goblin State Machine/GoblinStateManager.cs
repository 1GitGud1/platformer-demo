using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinStateManager : MonoBehaviour, IDamageable
{
    GoblinBaseState currentState;
    public GoblinIdleState idleState = new GoblinIdleState();
    public GoblinPursuingState pursuingState = new GoblinPursuingState();
    public GoblinShootingState shootingState = new GoblinShootingState();
    public GoblinDeadState deadState = new GoblinDeadState();

    public Rigidbody2D m_Rigidbody2D;
    public Animator animator;
    public Transform target;
    public Transform bow;
    public GameObject arrow;
    public LayerMask groundLayerMask;
    public KnockbackScript knockbackScript;

    public bool m_FacingRight = false;
    public float bowAngle;

    public int maxHealth = 20;
    int currentHealth;
    private float knockbackStr = 80;

    // Start is called before the first frame update
    void Start()
    {
        bow = transform.GetChild(0).transform;
        currentState = idleState;
        currentState.EnterState(this);

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(GoblinBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    public void FaceTarget()
    {
        if (transform.position.x < target.position.x)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = -1;
            transform.localScale = theScale;
            m_FacingRight = false;
        } else 
        {
            Vector3 theScale = transform.localScale;
            theScale.x = 1;
            transform.localScale = theScale;
            m_FacingRight = true;
        }
    }

    public void ShootArrow()
    {
        if (m_FacingRight)
        {
            GameObject projectile = Instantiate(arrow, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().AddForce(1f*GetVectorFromAngle(bowAngle+180), ForceMode2D.Impulse);
            animator.SetBool("Shooting", false);
            SwitchState(pursuingState);
        } else 
        {
            GameObject projectile = Instantiate(arrow, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>().AddForce(1f*GetVectorFromAngle(bowAngle), ForceMode2D.Impulse);
            animator.SetBool("Shooting", false);
            SwitchState(pursuingState);
        }
    }

    private static Vector3 GetVectorFromAngle(float angle) {
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public void TakeDamage(int DMG, Vector3 point)
    {
        currentHealth -= DMG;
        Debug.Log("Character dealt " + DMG + " damage");
        
        //Play hurt animation

        if(currentHealth <= 0)
        {
            SwitchState(deadState);
        }

        
        knockbackScript.knockback(point.x, knockbackStr);
    }
}
