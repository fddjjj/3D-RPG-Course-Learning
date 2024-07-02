using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button newGameButton;
    Button ContinueButton;
    Button exitButton;
    private void Awake()
    {
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        ContinueButton = transform.GetChild(2).GetComponent<Button>();
        exitButton = transform.GetChild(3).GetComponent<Button>();

        newGameButton.onClick.AddListener(NewGame);
        ContinueButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(QuitGame);
    }

    void NewGame() 
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
