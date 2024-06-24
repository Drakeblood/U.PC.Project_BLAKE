using _Project.Scripts;
using _Project.Scripts.Weapon;
using MBT;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyCharacter))]
public class AIController : MonoBehaviour
{
    public Weapon Weapon;

    [HideInInspector]
    public NavMeshAgent NavMeshAgent;

    private GameObject playerRef;
    private Animator animator;
    private EnemyCharacter character;

    public CombatStateReference CombatStateReference;
    public BoolReference HasLineOfSightReference;

    public Waypoints Waypoints;

    private void Awake()
    {
        UpdatePlayerRef();
        Weapon.Owner = gameObject.GetComponent<BlakeCharacter>();

        NavMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        character = GetComponent<EnemyCharacter>();

        character.onDeath += Die;
        GetComponent<EnemyFOV>().OnCanSeePlayerChanged += OnCanSeePlayerChanged;
    }

    private void Start()
    {
        ReferenceManager.BlakeHeroCharacter.onDeath += OnPlayerDeath;
    }
    
    private void OnDestroy()
    {
        ReferenceManager.BlakeHeroCharacter.onDeath -= OnPlayerDeath;
    }

    private void Die(BlakeCharacter blakeCharacter)
    {
        this.enabled = false;
        NavMeshAgent.isStopped = true;
        GetComponent<MBTExecutor>().enabled = false;
    }

    private void Update()
    {
        FaceThePlayer();

        animator.SetFloat("Direction", BlakeAnimatorHelper.CalculateDirection(NavMeshAgent.velocity, transform));
        animator.SetFloat("Speed", NavMeshAgent.velocity.magnitude);
    }

    public void UpdatePlayerRef()
    {
        playerRef = ReferenceManager.BlakeHeroCharacter?.gameObject;
    }

    public EnemyCharacter GetEnemyScript()
    {
        return character;
    }

    public void SetWaypoints(Waypoints waypoints)
    {
        this.Waypoints = waypoints;
    }

    private void FaceThePlayer()
    {
        if (playerRef == null)
        {
            return;
        }

        if (CombatStateReference.GetVariable().Value != CombatState.Patrol)
        {
            Vector3 direction = (playerRef.transform.position - gameObject.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, lookRotation, Time.deltaTime * 40f);
        }
    }

    private void OnCanSeePlayerChanged(bool newCanSeePlayer)
    {
        HasLineOfSightReference.Value = newCanSeePlayer;
        if (newCanSeePlayer)
        {
            CombatStateReference.GetVariable().Value = CombatState.Attack;
            if (IsInvoking("ClearPlayerFocus"))
            {
                CancelInvoke("ClearPlayerFocus");
            }
        }
        else
        {
            Invoke("ClearPlayerFocus", 10f);
        }
    }

    private void ClearPlayerFocus()
    {
        CombatStateReference.GetVariable().Value = CombatState.Patrol;
    }

    private void OnPlayerDeath(BlakeCharacter blakeCharacter)
    {
        ClearPlayerFocus();
    }
}
