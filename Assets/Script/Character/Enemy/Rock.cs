using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Rock : MonoBehaviour
{
    public enum RockStates {HitPlayer,HitGolem,HitNothing }
    private Rigidbody rb;
    public RockStates rockState;
    public GameObject breakEffect;
    [Header("Base Settings")]
    public float force;
    public GameObject target;
    private Vector3 direction;
    public int damage;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockState = RockStates.HitPlayer;
        FlyToTarget();
        
    }

    void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockState = RockStates.HitNothing;
        }
    }
    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force,ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (rockState)
        {
            case RockStates.HitPlayer:
                if(collision.gameObject.CompareTag("player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    collision.gameObject.GetComponent<Animator>().SetTrigger("dizzy");
                    collision.gameObject.GetComponent<ChararcterState>().TakeDamage(damage, collision.gameObject.GetComponent<ChararcterState>());

                    rockState = RockStates.HitNothing;
                }
                break;
            case RockStates.HitGolem:
                if(collision.gameObject.GetComponent<Golem>())
                {
                    var  collState  = collision.gameObject.GetComponent<ChararcterState>();
                    collState.TakeDamage(damage, collState);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
