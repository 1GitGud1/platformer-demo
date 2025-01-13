using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPursuingState : GoblinBaseState
{
    public float horizontalMove = 0f;
    float dangerDistance = 0.5f;
    float minimumDistance = 1f;
    float maximumDistance = 1.25f;
    float nextAttack = 0f;
    bool escaping;

    // track location for two seconds after losing sight
    float lastSeenTime;
    float pathRequestCooldown;

    public override void EnterState(GoblinStateManager goblin){
        if (goblin.GroundCheck()) {
            goblin.m_Rigidbody2D.AddForce(new Vector2(0f, 1f), ForceMode2D.Impulse);
        }
        escaping = false;
        //have to reset path otherwise if no new path is found then enemy will not start going to last seen position (if (goblin.path.Length == 0))
        goblin.path = new Node[0];
        lastSeenTime = Time.time;
    }

    public override void UpdateState(GoblinStateManager goblin){
        //Line of sight ray is cast
        float distance = Vector2.Distance(goblin.transform.position, goblin.target.position);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(goblin.transform.position, (goblin.target.position - goblin.transform.position), distance, goblin.groundLayerMask);

        if (raycastHit2D.collider == null && distance < 2f) 
        {
            goblin.StopCoroutine("FollowPath");

            if(distance > maximumDistance)
            {
                if(goblin.transform.position.x < goblin.target.position.x){
                    horizontalMove = 50f;
                } else {
                    horizontalMove = -50f;
                }
                goblin.animator.SetFloat("Speed", 1);
            } 
            else if(distance < minimumDistance)
            {
                if (distance < dangerDistance) {
                    if (!escaping && goblin.GroundCheck()){
                        goblin.StartCoroutine(Evade(goblin));
                    }
                }
                // else if(nextAttack < Time.time && raycastHit2D.collider == null){
                //     horizontalMove = 0;
                //     goblin.animator.SetFloat("Speed", 0);
                //     nextAttack = Time.time+5f;
                //     goblin.SwitchState(goblin.shootingState);
                // }
                else if(goblin.transform.position.x < goblin.target.position.x){
                    horizontalMove = -50f;
                } else {
                    horizontalMove = 50f;
                }
                goblin.animator.SetFloat("Speed", 1);
            } 
            else
            {
                //starts drawing bow if the player is in line of sight and enough time passed from last attack
                if(nextAttack < Time.time && raycastHit2D.collider == null){
                    horizontalMove = 0;
                    goblin.animator.SetFloat("Speed", 0);
                    nextAttack = Time.time+5f;
                    goblin.SwitchState(goblin.shootingState);
                }
            }

            
            if (goblin.GroundCheck()){
                WallCheck(goblin);
            }

            goblin.targetLastSeen = goblin.target.position;
            lastSeenTime = Time.time;
        }
        else
        {
            if (Time.time < lastSeenTime+1) {
                goblin.targetLastSeen = goblin.target.position;
            }

            //insert pathfinding to the last scene player location
            if (pathRequestCooldown > 1f) {
                PathRequestManager.RequestPath(goblin.transform.position, goblin.targetLastSeen, goblin.OnPathFound);
                pathRequestCooldown = 0;
            }

            //if path not found then go towards last scene player location
            Debug.Log(goblin.path.Length);
            if (goblin.path.Length == 0)
            {
                if(goblin.transform.position.x < goblin.targetLastSeen.x){
                    horizontalMove = 50f;
                } else {
                    horizontalMove = -50f;
                }
                goblin.animator.SetFloat("Speed", 1);

                if (goblin.GroundCheck()){
                    WallCheck(goblin);
                }
                if (Mathf.Abs(goblin.transform.position.x - goblin.targetLastSeen.x) < 0.1f || Time.time > lastSeenTime+5) {
                    if (Vector2.Distance(goblin.transform.position, goblin.targetLastSeen) < 0.05f){
                        goblin.SwitchState(goblin.idleState);
                    } else {
                        goblin.SwitchState(goblin.searchingState);
                    }
                }
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

        goblin.jumpCooldown += Time.deltaTime;
        pathRequestCooldown += Time.deltaTime;
    }

    public override void FixedUpdateState(GoblinStateManager goblin){
        goblin.m_Rigidbody2D.velocity = new Vector2(horizontalMove*Time.fixedDeltaTime, goblin.m_Rigidbody2D.velocity.y);
    }

    public override void OnCollisionEnter2D(GoblinStateManager goblin){

    }

    private IEnumerator Evade(GoblinStateManager goblin)
    {
        escaping = true;
        if(goblin.transform.position.x < goblin.target.position.x){
            horizontalMove = 50f;
        } else {
            horizontalMove = -50f;
        }
        goblin.jumpCooldown = 0;
        goblin.m_Rigidbody2D.velocity = new Vector2(goblin.m_Rigidbody2D.velocity.x, 0);
        goblin.m_Rigidbody2D.AddForce(new Vector2(0, 2.5f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(1);
        if(goblin.transform.position.x < goblin.target.position.x){
            horizontalMove = -50f;
        } else {
            horizontalMove = 50f;
        }
        yield return new WaitForSeconds(1);
        escaping = false;
    }

    private void WallCheck(GoblinStateManager goblin)
    {
        if (goblin.m_FacingRight){
            RaycastHit2D wallCheck = Physics2D.Raycast(goblin.transform.position, Vector2.left, 0.15f, goblin.groundLayerMask);
            if (wallCheck != false){
                goblin.jumpCooldown = 0;
                goblin.m_Rigidbody2D.AddForce(new Vector2(0, 3.25f), ForceMode2D.Impulse);
            }
        } else {
            RaycastHit2D wallCheck = Physics2D.Raycast(goblin.transform.position, Vector2.right, 0.15f, goblin.groundLayerMask);
            if (wallCheck != false){
                goblin.jumpCooldown = 0;
                goblin.m_Rigidbody2D.AddForce(new Vector2(0, 3.25f), ForceMode2D.Impulse);
            }
        }
    }
}
