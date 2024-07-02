using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : SingleTon<SceneController>, IEndGameObserve
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;
    public SceneFade sceneFadePrefab;

    bool fadeFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        GameManager.Instance.AddObserve(this);
        fadeFinished = true;
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType) 
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }else
        {
            player = GameManager.Instance.playerState.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
        
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
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadScene(SaveManager.Instance.SceneName));
    }
    public void TransitionToFirstScene()
    {
        StartCoroutine(LoadScene("SampleScene"));
    }

    public void TransitionToMain() 
    {
        StartCoroutine(LoadMain());
    }

    IEnumerator LoadScene(string sceneName)
    {
        SceneFade fade = Instantiate(sceneFadePrefab);
        if(sceneName != "")
        {
            yield return StartCoroutine(fade.FadeOut(1.5f));
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return player = Instantiate(playerPrefab,GameManager.Instance.GetEntrance().position,GameManager.Instance.GetEntrance().rotation);
            SaveManager.Instance.SavePlayerData();

            yield return StartCoroutine(fade.FadeIn(1.5f));
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        SceneFade fade = Instantiate(sceneFadePrefab);
        yield return StartCoroutine(fade.FadeOut(1.5f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(1.5f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
       
    }
}
