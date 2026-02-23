using UnityEngine;
using UnityEngine.AI;

public class Enemigo : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;

    Transform _player;

    [SerializeField] private Transform[] _patrolPoint;
    [SerializeField] private int _patrolIndex = 0;

    [SerializeField] private float _detectionRange = 7;
    [SerializeField] private float _attackRange = 1.8f;

    public enum EnemigoState 
    {
        Patrolling,
        Chasing,
        Searching,
        Attacking,
    }

    public EnemigoState _currentState;
    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        switch(_currentState)
        {
            case EnemigoState.Patrolling:
                Patrol();
                break;

            case EnemigoState.Chasing:
                Chase();
                break;

            case EnemigoState.Searching:

                Search();
                break;

            case EnemigoState.Attacking:

                Attack();
                break;

            default:
                Patrol();
                 break;
        }
        
    }

    bool OnRange()
    {
        if(Vector3.Distance(transform.position, _player.position) < _detectionRange)
        {
            return true;
        }
        else
        {
            return false;
        }
        
        
    }

    bool OnRange2(float distance)
        {
            if(Vector3.Distance(transform.position, _player.position) < distance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    void Patrol()
    {
        if(OnRange2(_detectionRange))
        {
            _currentState = EnemigoState.Chasing;
        }
    }
    void Chase()
    {
        if(OnRange2(_detectionRange))
        {
            _currentState = EnemigoState.Searching;
        }

        if(OnRange2(_attackRange))
        {
            _currentState = EnemigoState.Attacking;
            attackTimer = attackDelay;
        }

        _enemyAgent.SetDestination(_player.position);

    }

    float attackTimer;
    float attackDelay= 2;

    void Attack()
    {
        if (!OnRange2(_attackRange))
        {
            _currentState = EnemigoState.Chasing;
        }


        if(attackTimer < attackDelay)
        {
            attackTimer+= Time.deltaTime;
            
            return;
        }

        Debug.Log("Attack");
        attackTimer = 0;
    }
    void Search()
    {

    }


}
