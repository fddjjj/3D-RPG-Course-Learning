using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyControl
{
    [Header("skill")]
    public float kickForce;
    public void KickOff()
    {
        if(attackPlayer != null)
        {
            transform.LookAt(attackPlayer.transform);

            Vector3 dir = attackPlayer.transform.position - transform.position;
            dir.Normalize();
            attackPlayer.GetComponent<NavMeshAgent>().isStopped = true;
            attackPlayer.GetComponent <NavMeshAgent>().velocity = dir * kickForce;
            attackPlayer.GetComponent<Animator>().SetTrigger("dizzy");
        }
    }
}
