using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{

    public EnemyChaseState(NavMeshAgent navMeshAgent, GameObject playerRef) : base(navMeshAgent, playerRef) { }

    public override void EnterState(EnemyAIManager enemy)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(EnemyAIManager enemy)
    {
        throw new System.NotImplementedException();
    }

   
}
