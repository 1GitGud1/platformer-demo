using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPursuingState : GoblinBaseState
{
    float horizontalMove = 0f;
    float minimumDistance = 1f;
    float maximumDistance = 1.25f;
    float nextAttack = 0f;

    public override void EnterState(GoblinStateManager goblin){
        goblin.m_Rigidbody2D.AddForce(new Vector2(0f, 1f), ForceMode2D.Impulse);
    }

    public override void UpdateState(GoblinStateManager goblin){
        float distance = Vector2.Distance(goblin.transform.position, goblin.target.position);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(goblin.transform.position, (goblin.target.position - goblin.transform.position), distance, goblin.groundLayerMask);

        if(distance > maximumDistance)
        {
            if(goblin.transform.position.x < goblin.target.position.x){
                horizontalMove = 30f;
            } else {
                horizontalMove = -30f;
            }
            goblin.animator.SetFloat("Speed", 1);
        } else if(distance < minimumDistance)
        {
            if(goblin.transform.position.x < goblin.target.position.x){
                horizontalMove = -30f;
            } else {
                horizontalMove = 30f;
            }
            goblin.animator.SetFloat("Speed", 1);
        } else
        {
            horizontalMove = 0;
            goblin.animator.SetFloat("Speed", 0);
            if  (nextAttack < Time.time && raycastHit2D.collider == null){
                nextAttack = Time.time+5f;
                goblin.SwitchState(goblin.shootingState);
            }
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
    }

    public override void FixedUpdateState(GoblinStateManager goblin){
        goblin.m_Rigidbody2D.velocity = new Vector2(horizontalMove*Time.fixedDeltaTime, goblin.m_Rigidbody2D.velocity.y);
    }

    public override void OnCollisionEnter2D(GoblinStateManager goblin){

    }
}
