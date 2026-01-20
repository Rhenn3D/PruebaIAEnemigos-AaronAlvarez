using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;

    [Header("Patrol")]
    [SerializeField] private Transform[] _patrolPoints;
    private int _currentPatrolIndex;
    [SerializeField] private float _waitTimeAtPoint = 5f;
    private float _waitTimer;

    [Header("Detection")]
    private Transform _player;
    [SerializeField] private float _detectionRange = 6f;
    [SerializeField] private float _detectionAngle = 90f;

    [Header("Attack")]
    [SerializeField] private float _attackRange = 1.8f;
    [SerializeField] private float _attackCooldown = 1.5f;
    private float _attackTimer;

    [Header("Searching")]
    [SerializeField] private float _searchTime = 6f;
    [SerializeField] private float _searchRadius = 5f;
    private float _searchTimer;
    private Vector3 _lastPlayerPosition;

    public enum EnemyState
    {
        Patrolling,
        Waiting,
        Chasing,
        Searching,
        Attacking
    }

    public EnemyState currentState;

    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        currentState = EnemyState.Patrolling;
        _currentPatrolIndex = 0;
        SetNextPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;

            case EnemyState.Waiting:
                Waiting();
                break;

            case EnemyState.Chasing:
                Chase();
                break;

            case EnemyState.Searching:
                Search();
                break;

            case EnemyState.Attacking:
                Attack();
                break;

            default:
                Patrol();
                break;
        }
    }

    

    void Patrol()
    {
        if (PlayerInSight())
        {
            currentState = EnemyState.Chasing;
            return;
        }

        if (_enemyAgent.remainingDistance <= 0.5f)
        {
            currentState = EnemyState.Waiting;
            _waitTimer = 0f;
        }
    }

    void Waiting()
    {
        if (PlayerInSight())
        {
            currentState = EnemyState.Chasing;
            return;
        }

        _waitTimer += Time.deltaTime;

        if (_waitTimer >= _waitTimeAtPoint)
        {
            SetNextPatrolPoint();
            currentState = EnemyState.Patrolling;
        }
    }

    void Chase()
    {
        _enemyAgent.SetDestination(_player.position);
        _lastPlayerPosition = _player.position;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance <= _attackRange)
        {
            currentState = EnemyState.Attacking;
            _attackTimer = 0f;
        }
        else if (!PlayerInSight())
        {
            currentState = EnemyState.Searching;
            _searchTimer = 0f;
            _enemyAgent.SetDestination(_lastPlayerPosition);
        }
    }

    void Search()
    {
        if (PlayerInSight())
        {
            currentState = EnemyState.Chasing;
            return;
        }

        _searchTimer += Time.deltaTime;

        if (_enemyAgent.remainingDistance <= 0.5f)
        {
            Vector3 randomPoint;
            if (RandomPoint(_lastPlayerPosition, _searchRadius, out randomPoint))
            {
                _enemyAgent.SetDestination(randomPoint);
            }
        }

        if (_searchTimer >= _searchTime)
        {
            currentState = EnemyState.Patrolling;
            SetNextPatrolPoint();
        }
    }

    void Attack()
    {
        _enemyAgent.ResetPath();
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackCooldown)
        {
            Debug.Log("ENEMY ATTACK");

            _attackTimer = 0f;
            currentState = EnemyState.Chasing;
        }
    }

    

    void SetNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return;

        _enemyAgent.SetDestination(_patrolPoints[_currentPatrolIndex].position);

        _currentPatrolIndex++;
        if (_currentPatrolIndex >= _patrolPoints.Length)
            _currentPatrolIndex = 0;
    }

    bool PlayerInSight()
    {
        Vector3 directionToPlayer = _player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        if (distance > _detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > _detectionAngle * 0.5f)
            return false;

        return true;
    }

    bool RandomPoint(Vector3 center, float radius, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * radius;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, radius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
