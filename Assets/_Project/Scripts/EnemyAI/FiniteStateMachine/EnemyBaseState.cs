using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBaseState
{
    protected NavMeshAgent navMeshAgent;
    protected GameObject playerRef;

    public EnemyBaseState(NavMeshAgent navMeshAgent, GameObject playerRef)
    {
        this.navMeshAgent = navMeshAgent;
        this.playerRef = playerRef;
    }

    public abstract void EnterState(EnemyAIManager enemy);
    public abstract void UpdateState(EnemyAIManager enemy);
}
