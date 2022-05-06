using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Mirror.Scene]
    [SerializeField]
    private string[] scenes;

    private void Start()
    {
        foreach (string scene in scenes)
        {
            bool isLoaded = false;
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).path == scene)
                    isLoaded = true;
            if (!isLoaded)
                UnityEngine.SceneManagement.SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }
}
