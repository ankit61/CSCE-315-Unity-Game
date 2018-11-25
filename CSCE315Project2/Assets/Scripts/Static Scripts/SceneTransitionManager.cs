using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionManager
{

    public static void LoadScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

}