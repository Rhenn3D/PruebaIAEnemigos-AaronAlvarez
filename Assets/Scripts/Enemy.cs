using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent _enemyAgent;

    [SerializeField] private Transform[] _patrolPoints;

    public enum EnemyState
    {
        Patrolling,

        Chasing,

        Searching
    }

    public EnemyState currentState;

    



    void Awake()
    {
        _enemyAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
            break;

            case EnemyState.Chasing:
                Chase();
            break;

            case EnemyState.Searching:
                Search();
            break;

            default:
                Patrol();
            break;
        }
    }

    void Patrol()
    {
        
    }


    void Chase()
    {
        
    }

    void Search()
    {
        
    }
}
