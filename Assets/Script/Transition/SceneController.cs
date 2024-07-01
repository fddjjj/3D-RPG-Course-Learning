using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : SingleTon<SceneController>
{
    GameObject player;
    NavMeshAgent playerAgent;
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType) 
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                break;
        }
    }

    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        player = GameManager.Instance.playerState.gameObject;
        playerAgent = player.GetComponent<NavMeshAgent>();
        playerAgent.enabled = false;
        player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
        playerAgent.enabled = true;
        yield return null;
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag) 
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        for(int i= 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }

        return null;
    }
}