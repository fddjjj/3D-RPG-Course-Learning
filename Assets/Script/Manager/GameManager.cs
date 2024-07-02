using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cinemachine;

public class GameManager : SingleTon<GameManager>
{
    public ChararcterState playerState;
    List<IEndGameObserve> endGameObserve = new List<IEndGameObserve>();
    private CinemachineFreeLook followCamera;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public void RigistrPlayer(ChararcterState player)
    {
        playerState = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if(followCamera != null )
        {
            followCamera.Follow = playerState.transform.GetChild(2);
            followCamera.LookAt = playerState.transform.GetChild(2);
        }
    }
    public void AddObserve(IEndGameObserve observe)
    {
        endGameObserve.Add(observe);
    }
    public void RemoveObserve(IEndGameObserve observe)
    {
        endGameObserve.Remove(observe);
    }

    public void NotifyObserve()
    {
        foreach (var observe in endGameObserve)
        {
            observe.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())
        {
            if(item.destinationTag  == TransitionDestination.DestinationTag.Enter)
            {
                return item.transform;
            }
        }
        return null;
    }
}
