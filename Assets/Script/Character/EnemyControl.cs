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

    private ChararcterState chararcterState;

    [Header("Base Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    private GameObject attackPlayer;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;

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
        chararcterState = GetComponent<ChararcterState>();
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
        lastAttackTime -= Time.deltaTime;
    }

    void SwithAnimation()
    {
        anim.SetBool("walk", iswalk);
        anim.SetBool("chase", ischase);
        anim.SetBool("follow", isfollow);
        anim.SetBool("critical", chararcterState.isCritical);
    }
    void SwithState()
    {
        //find player,changeto chase;
        if (FoundPlayer())
        {
            enemyState = EnemyState.chase;
            //Debug.Log("find");
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
                    agent.isStopped = false;
                    if (Vector3.Distance(agent.destination, transform.position) < 1 && Vector3.Distance(transform.position,attackPlayer.transform.position)  > agent.stoppingDistance) 
                    {
                        agent.destination = attackPlayer.transform.position;
                    }
                    //agent.destination = attackPlayer.transform.position;

                    
                }
                //¹¥»÷
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    //Debug.Log("start Attack");
                    isfollow =false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = chararcterState.attackDataSo.coolDown;
                        //±©»÷ÅÐ¶Ï
                        chararcterState.isCritical = Random.value < chararcterState.attackDataSo.criticalChance;
                        //Ö´ÐÐ¹¥»÷
                        Attack();
                    }
                }
                break;
            case EnemyState.dead:
                break;
        }
    }
    void Attack()
    {
        transform.LookAt(attackPlayer.transform);
        if (TargetInAttackRange())
        {
            anim.SetTrigger("attack");
        }
        if(TargetInSkillRange())
        {
           // anim.SetTrigger("skill");
        }
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

    bool TargetInAttackRange()
    {
        if(attackPlayer != null)
        {
            return Vector3.Distance(attackPlayer.transform.position, transform.position) <= chararcterState.attackDataSo.attackRange;
        }else
        { return false; }    
    }

    bool TargetInSkillRange() 
    {
        if(attackPlayer != null)
        {
            return Vector3.Distance(attackPlayer.transform.position, transform.position) <= chararcterState.attackDataSo.skillRange;
        }else
        { return false; }
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
