using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { guard, patrol, chase, dead }
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyControl : MonoBehaviour
{
    private EnemyState enemyState;
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Base Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    private GameObject attackPlayer;
    public float lookAtTime;
    private float remainLookAtTime;

    [Header("¶¯»­ÇÐ»»²ÎÊý")]
    bool iswalk;
    bool ischase;
    bool isfollow;

    [Header("Patrol State")]
    public float partrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
        guardPosition = transform.position;
        remainLookAtTime = lookAtTime;
    }
    private void Start()
    {
        if(isGuard)
        {
            enemyState = EnemyState.guard;
        }else
        {
            enemyState = EnemyState.patrol;
            GetNewWayPoint();
           // Debug.Log(transform.position +  "  "  + wayPoint);
        }    
    }

    private void Update()
    {
        SwithState();
        SwithAnimation();
    }

    void SwithAnimation()
    {
        anim.SetBool("walk", iswalk);
        anim.SetBool("chase", ischase);
        anim.SetBool("follow", isfollow);
    }
    void SwithState()
    {
        //find player,changeto chase;
        if (FoundPlayer())
        {
            enemyState = EnemyState.chase;
            //Debug.Log("find");
            //StartCoroutine(waitFor());
        }
        switch (enemyState)
        {
            case EnemyState.guard:
                break;
            case EnemyState.patrol:
                ischase = false;
                agent.speed = speed * 0.5f;
              //  Debug.Log(transform.position);
                if(Vector3.Distance(wayPoint,transform.position) <= agent.stoppingDistance)
                {
                    iswalk = false;
                    if(remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }else
                {
                    iswalk=true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyState.chase:
                iswalk = false;
                ischase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isfollow = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                        agent.destination = transform.position;
                    }
                    else if (isGuard)
                        enemyState = EnemyState.guard;
                    else
                        enemyState = EnemyState.patrol;
                    
                }
                else
                {
                    isfollow = true;
                   // var ispath = agent.CalculatePath(attackPlayer.transform.position, agent.path);
                  //  if(ispath)
                   // {
                   //     agent.SetPath(agent.path);
                //    }
                    if (Vector3.Distance(agent.destination, transform.position) < 1)
                    {
                        agent.destination = attackPlayer.transform.position;
                    }
                    //agent.destination = attackPlayer.transform.position;
                    //StartCoroutine(MoveEnemyToPlayer());
                }
                break;
            case EnemyState.dead:
                break;
        }
    }
    IEnumerator waitFor()
    {
        yield return new WaitForSeconds(100f);
    }
    IEnumerator MoveEnemyToPlayer()
    {
        agent.destination = attackPlayer.transform.position;
        yield return new WaitForSeconds(0.11f);
    }
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("player"))
            {
                attackPlayer = target.gameObject;
                return true;
            }

        }
        attackPlayer = null;
        return false;
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomx = Random.Range(-partrolRange, partrolRange);
        float randomz = Random.Range(-partrolRange, partrolRange);

        Vector3 randomPoint = new Vector3(guardPosition.x + randomx,transform.position.y,guardPosition.z + randomz);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, partrolRange, 1) ? hit.position : transform.position; 
    }
         
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
