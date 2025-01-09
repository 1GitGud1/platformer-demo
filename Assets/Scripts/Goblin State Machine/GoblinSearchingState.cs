using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSearchingState : GoblinBaseState
{
    public float horizontalMove = 50f;
    float randomCooldown = 0f;
    float randomStartTime;

    public override void EnterState(GoblinStateManager goblin)
    {
        randomStartTime = Time.time;
    }

    public override void UpdateState(GoblinStateManager goblin)
    {
        float distance = Vector2.Distance(goblin.transform.position, goblin.target.position);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(goblin.transform.position, (goblin.target.position - goblin.transform.position), distance, goblin.groundLayerMask);

        if (raycastHit2D.collider == null && distance < 2f) 
        {
            goblin.SwitchState(goblin.pursuingState);
        } else if (randomCooldown > 0.3f) {
            int direction = Random.Range(1, 11);
            if(direction < 7) {
                horizontalMove = (goblin.transform.position.x - goblin.targetLastSeen.x)/Mathf.Abs(goblin.transform.position.x - goblin.targetLastSeen.x) * -50;
            } else if(direction > 8 && goblin.GroundCheck()) {
                goblin.jumpCooldown = 0;
                goblin.m_Rigidbody2D.AddForce(new Vector2(0, 3.25f), ForceMode2D.Impulse);
            } else {
                horizontalMove = (goblin.transform.position.x - goblin.targetLastSeen.x)/Mathf.Abs(goblin.transform.position.x - goblin.targetLastSeen.x) * 50;
            }
            randomCooldown = 0f;
        }

        if (Time.time > randomStartTime+5f) {
            goblin.SwitchState(goblin.idleState);
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

        goblin.jumpCooldown += Time.deltaTime;
        randomCooldown += Time.deltaTime;
    }

    public override void FixedUpdateState(GoblinStateManager goblin)
    {
        goblin.m_Rigidbody2D.velocity = new Vector2(horizontalMove*Time.fixedDeltaTime, goblin.m_Rigidbody2D.velocity.y);
    }

    public override void OnCollisionEnter2D(GoblinStateManager goblin)
    {

    }
}
