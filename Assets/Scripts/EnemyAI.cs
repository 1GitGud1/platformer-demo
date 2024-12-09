using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    public Animator animator;
    public LayerMask groundLayer;

    public int monsterDamage;
    public float speed;
    public float minimumDistance;
    private bool m_FacingRight = false;
    float jumpDirection;
    private bool attacking = false;
    private float jumpCooldown;
    private bool enableRandomMove = false;

    public Transform target;
    Node[] path;
    int targetIndex;
    float pathRequestCooldown;


    // Start is called before the first frame update
    void Start()
    {
        // Invoke("DelayedStart", 5f);
        //StartCoroutine("RandomMove");
    }

    // void DelayedStart()
    // {
    //     PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        
    // }

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
            if (pathRequestCooldown > 1f) {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                pathRequestCooldown = 0;
            }
            
            if (enableRandomMove) {
                StartCoroutine("RandomMove");
            }

            animator.SetFloat("Speed", 1);
        } else if(!attacking)
        {
            //StopCoroutine("FollowPath");
            animator.SetFloat("Speed", 0);
            attacking = true;
            StartCoroutine(Attack());
        }
        
        if (speed > 0 && !m_FacingRight)
		{
			// ... flip the player.
	    	Flip();
		}
		else if (speed < 0 && m_FacingRight)
		{
			// ... flip the player.
			Flip();
		}

        m_Rigidbody2D.velocity = new Vector2(speed*Time.deltaTime, m_Rigidbody2D.velocity.y);

        jumpCooldown += Time.deltaTime;
        pathRequestCooldown += Time.deltaTime;
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
        yield return new WaitForSeconds(0.25f);
        attacking = false;
    }

    public void OnPathFound(Node[] newPath, bool pathSuccessful) 
    {
        if (pathSuccessful) {
            path = newPath;

            enableRandomMove = false;
            StopCoroutine("RandomMove");

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        } 
        
    }

    IEnumerator FollowPath() 
    {
        Vector3 currentWaypoint = path[0].worldPosition;
        targetIndex = 0;

        while (true) {
            if (Vector2.Distance(transform.position,currentWaypoint) < 0.1f) {
                targetIndex++;
                if (targetIndex >= path.Length) {
                    targetIndex = 0;
                    path = new Node[0];

                    enableRandomMove = true;

                    yield break;
                }
                currentWaypoint = path[targetIndex].worldPosition;
            }

            //movement towards nodes logic
            if (path[targetIndex].jumpToNode && GroundCheck()){
                jumpCooldown = 0;
                m_Rigidbody2D.velocity = new Vector2(speed*Time.deltaTime, 0);
                yield return new WaitForSeconds(0.1f);
                m_Rigidbody2D.AddForce(new Vector2(0, 3.25f), ForceMode2D.Impulse);
            }
            if(transform.position.x < currentWaypoint.x){
                speed = 90;
            } 
            else if (Mathf.Abs(transform.position.x - currentWaypoint.x) < 0.02f) {
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }
            else {
                speed = -90;
            }

            //transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed*Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RandomMove()
    {
        enableRandomMove = false;
            int direction = Random.Range(1, 11);
            //Debug.Log(direction);
            if(direction < 7) {
                speed = (transform.position.x - target.position.x)/Mathf.Abs(transform.position.x - target.position.x) * -90;
            } else if(direction > 8 && GroundCheck()) {
                jumpCooldown = 0;
                m_Rigidbody2D.AddForce(new Vector2(0, 3.25f), ForceMode2D.Impulse);
            } else {
                speed = (transform.position.x - target.position.x)/Mathf.Abs(transform.position.x - target.position.x) * 90;
            }

            yield return new WaitForSeconds(0.3f);
        enableRandomMove = true;
    }

    private bool GroundCheck()
    {
        if (jumpCooldown >= 0.3f) {
            RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.15f, groundLayer);
            if (groundCheck.collider != null) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i++) {
                Gizmos.color = Color.black;
                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i].worldPosition);
                } 
                else {
                    Gizmos.DrawLine(path[i-1].worldPosition, path[i].worldPosition);
                }
            }
        }
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y-0.15f, transform.position.z));
    }
}
