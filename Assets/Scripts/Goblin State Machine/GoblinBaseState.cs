using UnityEngine;

public abstract class GoblinBaseState
{
    public abstract void EnterState(GoblinStateManager goblin);

    public abstract void UpdateState(GoblinStateManager goblin);

    public abstract void FixedUpdateState(GoblinStateManager goblin);

    public abstract void OnCollisionEnter2D(GoblinStateManager goblin);
}
