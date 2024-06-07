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
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
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
            anim.SetTrigger("attack");
            attackDuringTime = 1f;
        }
    }
}
