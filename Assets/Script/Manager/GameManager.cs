using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public ChararcterState playerState;
    List<IEndGameObserve> endGameObserve = new List<IEndGameObserve>();
    public void RigistrPlayer(ChararcterState player)
    {
        playerState = player;
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
}
