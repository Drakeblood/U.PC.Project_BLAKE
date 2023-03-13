using UnityEngine;
using UnityEngine.AI;

public class EnemyAIManager : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private GameObject _playerRef;
    private Transform _groundCenterPoint;

    public EnemyBaseState currentState;

    public EnemyPatrolState PatrolState;
    public EnemyChaseState ChaseState;
    public EnemyAttackState AttackState;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerRef = GameObject.FindGameObjectWithTag("Player");
        _groundCenterPoint = GameObject.Find("Floor").transform;
    }

    private void OnEnable()
    {
        PatrolState = new EnemyPatrolState(_navMeshAgent, _playerRef, _groundCenterPoint);
        ChaseState = new EnemyChaseState(_navMeshAgent, _playerRef);
        AttackState = new EnemyAttackState(_navMeshAgent, _playerRef);
    }

    private void Start()
    {
        currentState = PatrolState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }
}
