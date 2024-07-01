using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private ChararcterState chararcterState;

    private GameObject attackTarget;
    private float attackDuringTime;
    private bool isdead;
    private float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        chararcterState = GetComponent<ChararcterState>();
        stopDistance = agent.stoppingDistance;
    }
    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += AttackEvent;
    }

    private void Start()
    {
        GameManager.Instance.RigistrPlayer(chararcterState);
    }

    private void OnDisable()
    {
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= AttackEvent;
    }

    private void Update()
    {
        isdead = chararcterState.currentHealth == 0;
        if (isdead)
        {
            GameManager.Instance.NotifyObserve();
        }
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
        anim.SetBool("dead", isdead);
        if (attackDuringTime >= 0)
        {
            attackDuringTime -= Time.deltaTime;
        }
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isdead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
       // Debug.Log(agent.destination);
    }  
    private void AttackEvent(GameObject target)
    {
        if (isdead) return;
        if (target != null)
        {
            attackTarget = target;
            chararcterState.isCritical = UnityEngine.Random.value < chararcterState.attackDataSo.criticalChance;
            StartCoroutine(MoveToAttackPosition());
            agent.destination = attackTarget.transform.position;
        }
        
    }

    IEnumerator MoveToAttackPosition()
    {
        agent.isStopped = false;
        agent.stoppingDistance = chararcterState.attackDataSo.attackRange;
        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(attackTarget.transform.position,transform.position) > chararcterState.attackDataSo.attackRange )
        {
            // Debug.Log(attackTarget.transform.position+"   " + transform.position);
            
            // Debug.Log(agent.destination);
            yield return null;
        }
        agent.isStopped = true;

        if(attackDuringTime <= 0)
        {
            
            anim.SetBool("critical", chararcterState.isCritical);
            anim.SetTrigger("attack");
            attackDuringTime = chararcterState.attackDataSo.coolDown;
        }
    }


    //Animation Event;
    void Hit()
    {
        if (attackTarget.CompareTag("attackable"))
        {
            if(attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockState == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockState = Rock.RockStates.HitGolem;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetState = attackTarget.GetComponent<ChararcterState>();
            targetState.TakeDamage(chararcterState, targetState);
        }
        
    }
}
