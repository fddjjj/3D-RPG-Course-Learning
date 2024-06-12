using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { guard, patrol, chase, dead }
[RequireComponent(typeof(NavMeshAgent),typeof(ChararcterState))]
public class EnemyControl : MonoBehaviour,IEndGameObserve
{
    private EnemyState enemyState;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;

    private ChararcterState chararcterState;

    [Header("Base Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    protected GameObject attackPlayer;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private quaternion guardRotation;
    private bool isPlayerDead;


    [Header("¶¯»­ÇÐ»»²ÎÊý")]
    bool iswalk;
    bool ischase;
    bool isfollow;
    bool isdead;

    [Header("Patrol State")]
    public float partrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        chararcterState = GetComponent<ChararcterState>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPosition = transform.position;
        guardRotation = transform.rotation;
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
    private void OnEnable()
    {
        GameManager.Instance.AddObserve(this);
    }

    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserve(this);
    }

    private void Update()
    {   
        if (chararcterState.currentHealth <= 0)
            isdead = true;
        if (!isPlayerDead)
        {
            SwithState();
            SwithAnimation();
            lastAttackTime -= Time.deltaTime;
            
        }
        
    }

    void SwithAnimation()
    {
        anim.SetBool("walk", iswalk);
        anim.SetBool("chase", ischase);
        anim.SetBool("follow", isfollow);
        anim.SetBool("critical", chararcterState.isCritical);
        anim.SetBool("dead", isdead);
    }
    void SwithState()
    {
        if (isdead)
        {
            enemyState = EnemyState.dead;
        }
        //find player,changeto chase;
        else if (FoundPlayer())
        {
            enemyState = EnemyState.chase;
            //Debug.Log("find");
        }
        switch (enemyState)
        {
            case EnemyState.guard:
                ischase = false;
                if(transform.position != guardPosition)
                {
                    iswalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPosition;
                    if(Vector3.SqrMagnitude(guardPosition - transform.position) <= agent.stoppingDistance)
                    {
                        iswalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                        remainLookAtTime = lookAtTime;
                    }
                        

                }
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
                        chararcterState.isCritical = UnityEngine.Random.value < chararcterState.attackDataSo.criticalChance;
                        //Ö´ÐÐ¹¥»÷
                        Attack();
                    }
                }
                break;
            case EnemyState.dead:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
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
            anim.SetTrigger("skill");
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
        float randomx = UnityEngine.Random.Range(-partrolRange, partrolRange);
        float randomz = UnityEngine.Random.Range(-partrolRange, partrolRange);

        Vector3 randomPoint = new Vector3(guardPosition.x + randomx,transform.position.y,guardPosition.z + randomz);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, partrolRange, 1) ? hit.position : transform.position; 
    }
         
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //animation event
    void Hit()
    {
        if(attackPlayer != null)
        {
            var targetState = attackPlayer.GetComponent<ChararcterState>();
            targetState.TakeDamage(chararcterState, targetState);
        }
    }

    public void EndNotify()
    {
        //»ñÊ¤¶¯»­
        //Í£Ö¹ÒÆ¶¯
        ischase = false;
        iswalk = false;
        attackPlayer = null;
        anim.SetBool("win", true);
        isPlayerDead = true;

    }
}
