using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    public Animator animator;

    public int monsterDamage;
    public float speed;
    public Transform target;
    public float minimumDistance;
    private bool m_FacingRight = false;
    float jumpDirection;
    private bool attacking = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterStats>().TakeDamage(monsterDamage, transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, target.position) > minimumDistance)
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(target.position.x, transform.position.y), speed * Time.deltaTime);
            animator.SetFloat("Speed", 1);
        } else if(!attacking)
        {
            animator.SetFloat("Speed", 0);
            attacking = true;
            StartCoroutine(Attack());
        }
        
        if ((target.position.x - transform.position.x) > 0 && !m_FacingRight)
		{
			// ... flip the player.
	    	Flip();
		}
		else if ((target.position.x - transform.position.x) < 0 && m_FacingRight)
		{
			// ... flip the player.
			Flip();
		}
    }

    private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(1);
        m_Rigidbody2D.AddForce(new Vector2((m_FacingRight ? 1f : -1f), 2f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        attacking = false;
    }
}
