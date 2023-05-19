using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public static int nAgents;

    public void OnButtonPressStart()
    {
        SceneManager.LoadScene("MainScene");
        SceneManager.UnloadScene("MainMenu");
    }

    public void OnButtonPressExit()
    {
        Application.Quit();
    }

    public void setNagents(string _nAgents)
    {
        nAgents = int.Parse(_nAgents);
    }
}
