using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;


public class MainMenu : MonoBehaviour
{
    Button newGameButton;
    Button ContinueButton;
    Button exitButton;

    PlayableDirector director;

    private void Awake()
    {
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        ContinueButton = transform.GetChild(2).GetComponent<Button>();
        exitButton = transform.GetChild(3).GetComponent<Button>();

        newGameButton.onClick.AddListener(PlayTimeline);
        ContinueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    void PlayTimeline()
    {
        director.Play();
    }
    void NewGame(PlayableDirector obj) 
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstScene();
    }

    void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
    private void QuitGame()
    {
        Application.Quit();
        Debug.Log("quit");
    }
}
