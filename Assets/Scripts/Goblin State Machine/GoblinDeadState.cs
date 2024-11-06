using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinDeadState : GoblinBaseState
{
    public override void EnterState(GoblinStateManager goblin)
    {
        Debug.Log("Enemy died");

        goblin.animator.SetBool("isDead", true);

        int LayerTransparentFX = LayerMask.NameToLayer("TransparentFX");
        goblin.gameObject.layer = LayerTransparentFX;
    }

    public override void UpdateState(GoblinStateManager goblin)
    {

    }

    public override void FixedUpdateState(GoblinStateManager goblin)
    {

    }

    public override void OnCollisionEnter2D(GoblinStateManager goblin)
    {

    }
}
