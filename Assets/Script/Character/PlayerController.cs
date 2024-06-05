using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        MouseManager.instance.OnMouseClicked += MoveToTarget;
    }

    private void Update()
    {
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
    }
    public void MoveToTarget(Vector3 target)
    {
        agent.destination = target;
    }
}
