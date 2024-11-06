using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinIdleState : GoblinBaseState
{
    float horizontalMove = 20f;
    float moveSpeed;
    //float timePassed = 0f;
    bool patrolling = false;
    
    public override void EnterState(GoblinStateManager goblin){
        goblin.animator.SetFloat("Speed", 1);
    }

    public override void UpdateState(GoblinStateManager goblin){
        if (!patrolling){
            goblin.StartCoroutine(Patrol(goblin));
        }

        if (horizontalMove < 0 && !goblin.m_FacingRight)
		{
			// ... flip the player.
	    	goblin.Flip();
		}
		else if (horizontalMove > 0 && goblin.m_FacingRight)
		{
			// ... flip the player.
			goblin.Flip();
		}

        //only detects player if they are facing them (to be removed and implemented into sneaking functionality)
        if ((goblin.target.position.x < goblin.transform.position.x && goblin.m_FacingRight) || (goblin.transform.position.x < goblin.target.position.x && !goblin.m_FacingRight))
        {
            //Line of sight ray is cast
            float distanceToPlayer = Vector2.Distance(goblin.transform.position, goblin.target.position);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(goblin.transform.position, (goblin.target.position - goblin.transform.position), distanceToPlayer, goblin.groundLayerMask);
            if (raycastHit2D.collider == null && distanceToPlayer < 2f){
                goblin.StopCoroutine("Patrol");
                goblin.SwitchState(goblin.pursuingState);
            }
        }
    }

    public override void FixedUpdateState(GoblinStateManager goblin){
        goblin.m_Rigidbody2D.velocity = new Vector2(horizontalMove*Time.fixedDeltaTime, goblin.m_Rigidbody2D.velocity.y);
    }

    public override void OnCollisionEnter2D(GoblinStateManager goblin){

    }

    private IEnumerator Patrol(GoblinStateManager goblin)
    {
        patrolling = true;
        yield return new WaitForSeconds(2);
        moveSpeed = horizontalMove;
        goblin.animator.SetFloat("Speed", 0);
        horizontalMove = 0;
        yield return new WaitForSeconds(2);
        goblin.animator.SetFloat("Speed", 1);
        horizontalMove = moveSpeed*-1;
        patrolling = false;
    }

}
