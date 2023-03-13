using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
    private float _randomPointRange = 100.0f;

    private Transform _groundCenterPoint;

    public EnemyPatrolState(NavMeshAgent navMeshAgent, GameObject playerRef, Transform groundCenterPoint) : base(navMeshAgent, playerRef) 
    {
        _groundCenterPoint = groundCenterPoint;
    }

    public override void EnterState(EnemyAIManager enemy)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(EnemyAIManager enemy)
    {
        SetRandomPosition(_groundCenterPoint.position, _randomPointRange);
    }

    private void SetRandomPosition(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                MoveToPosition(hit.position);
            }
        }
    }

    private void MoveToPosition(Vector3 movePosition)
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.SetDestination(movePosition);
        }
    }
}
