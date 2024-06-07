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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        chararcterState = GetComponent<ChararcterState>();
    }

    private void Start()
    {
        MouseManager.instance.OnMouseClicked += MoveToTarget;
        MouseManager.instance.OnEnemyClicked += AttackEvent;
    }



    private void Update()
    {
        isdead = chararcterState.currentHealth == 0;
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
        agent.isStopped = false;
        agent.destination = target;
       // Debug.Log(agent.destination);
    }  
    private void AttackEvent(GameObject target)
    {
        if(target != null)
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
        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(attackTarget.transform.position,transform.position) > chararcterState.attackDataSo.attackRange + attackTarget.GetComponent<NavMeshAgent>().radius)
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
        var targetState = attackTarget.GetComponent<ChararcterState>();
        targetState.TakeDamage(chararcterState, targetState);
    }
}
