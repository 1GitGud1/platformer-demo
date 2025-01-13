using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinStateManager : MonoBehaviour, IDamageable
{
    GoblinBaseState currentState;
    public GoblinIdleState idleState = new GoblinIdleState();
    public GoblinPursuingState pursuingState = new GoblinPursuingState();
    public GoblinShootingState shootingState = new GoblinShootingState();
    public GoblinSearchingState searchingState = new GoblinSearchingState();
    public GoblinDeadState deadState = new GoblinDeadState();

    public Rigidbody2D m_Rigidbody2D;
    public Animator animator;
    public Transform target;
    public Vector3 targetLastSeen;
    public Transform bow;
    public GameObject arrow;
    public LayerMask groundLayerMask;
    public KnockbackScript knockbackScript;

    public bool m_FacingRight = false;
    public float bowAngle;

    public float jumpCooldown;

    public int maxHealth = 20;
    int currentHealth;
    private float knockbackStr = 80;

    public Node[] path;
    int targetIndex;

    // Start is called before the first frame update
    void Start()
    {
        bow = transform.GetChild(0).transform;
        currentState = idleState;
        currentState.EnterState(this);

        currentHealth = maxHealth;
        path = new Node[0];
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
        
        targetLastSeen = target.position;

        if(currentHealth <= 0)
        {
            SwitchState(deadState);
        } 
        else if (currentState == idleState) 
        {
            SwitchState(pursuingState);
        }

        
        knockbackScript.knockback(point.x, knockbackStr);
    }

    public bool GroundCheck()
    {
        if (jumpCooldown >= 0.3f) {
            RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.15f, groundLayerMask);
            if (groundCheck.collider != null) {
                //Debug.Log("goblin jump check");
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public void OnPathFound(Node[] newPath, bool pathSuccessful) 
    {
        if (pathSuccessful) {
            path = newPath;

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        } 
        
    }

    public IEnumerator FollowPath()
    {
        animator.SetFloat("Speed", 1);
        Vector3 currentWaypoint = path[0].worldPosition;
        targetIndex = 0;

        while (true) {
            if (Vector2.Distance(transform.position,currentWaypoint) < 0.1f) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    path = new Node[0];

                    SwitchState(idleState);

                    yield break;
                }
                currentWaypoint = path[targetIndex].worldPosition;
            }

            //movement towards nodes logic
            //Debug.Log(path.Length + " is the length of array");
            Debug.Log(targetIndex + " is the index searched");
            if (path[targetIndex].jumpToNode && GroundCheck()){
                jumpCooldown = 0;
                m_Rigidbody2D.velocity = new Vector2(pursuingState.horizontalMove*Time.deltaTime, 0);
                yield return new WaitForSeconds(0.1f);
                m_Rigidbody2D.AddForce(new Vector2(0, 3.25f), ForceMode2D.Impulse);
            }
            if(transform.position.x < currentWaypoint.x){
                pursuingState.horizontalMove = 50;
            } 
            else if (Mathf.Abs(transform.position.x - currentWaypoint.x) < 0.02f) {
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
            else {
                pursuingState.horizontalMove = -50;
            }

            yield return null;
        }
    }
}
