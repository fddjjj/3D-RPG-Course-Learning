using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Golem : EnemyControl
{

    [Header("skill")]
    public float kickForce;

    public void KickOff()
    {
        if (attackPlayer != null && transform.IsFacingTarget(attackPlayer.transform))
        {
            var targetState = attackPlayer.GetComponent<ChararcterState>();
            Vector3 direction = attackPlayer.transform.position - transform.position;
            direction.Normalize();
            targetState.GetComponent<NavMeshAgent>().isStopped = true;
            targetState.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            targetState.GetComponent<Animator>().SetTrigger("Dizzy");
            targetState.TakeDamage(chararcterState, targetState);
        }
    }
}
