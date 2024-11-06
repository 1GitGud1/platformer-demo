using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinShootingState : GoblinBaseState
{

    public override void EnterState(GoblinStateManager goblin)
    {
        goblin.FaceTarget();
        goblin.m_Rigidbody2D.velocity = new Vector2(0, goblin.m_Rigidbody2D.velocity.y);
        goblin.animator.SetBool("Shooting", true);
        goblin.bowAngle = 0;
    }

    public override void UpdateState(GoblinStateManager goblin)
    {
        float distance = Vector2.Distance(goblin.transform.position, goblin.target.position);
        //angle from goblin to player is calculated and bow rotation is applied accordingly
        //Mathf.Atan2() returns values between 180 and -180
        float angleRad = Mathf.Atan2(goblin.target.position.x-goblin.transform.position.x, goblin.target.position.y-goblin.transform.position.y);
        //have to account for goblin facing direction
        float targetAngle = (angleRad/(Mathf.PI/180f))*-1 + (-90 + -7 * distance)*goblin.transform.localScale.x;
        //Debug.Log((angleRad/(Mathf.PI/180f)));
        goblin.bowAngle += (targetAngle-goblin.bowAngle)/32;
        goblin.bow.transform.rotation = Quaternion.Euler(0, 0, goblin.bowAngle);
    }

    public override void FixedUpdateState(GoblinStateManager goblin)
    {

    }

    public override void OnCollisionEnter2D(GoblinStateManager goblin)
    {

    }
}
