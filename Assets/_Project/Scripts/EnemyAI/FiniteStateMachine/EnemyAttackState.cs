using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(NavMeshAgent navMeshAgent, GameObject playerRef) : base(navMeshAgent, playerRef) { }

    public override void EnterState(EnemyAIManager enemy)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(EnemyAIManager enemy)
    {
        throw new System.NotImplementedException();
    }
}
