using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType { SameScene,DifferentScene}
    [Header("Transition Info")]
    public string sceneName;
    public TransitionType  transitionType;
    public TransitionDestination.DestinationTag destinationTag;


    private bool canTrans;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            //TODO:´«ËÍ
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("player"))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("player"))
        {
            canTrans = false;
        }
    }
}
