using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    private int currentSceneNR;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentSceneNR = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentSceneNR == 0)
                currentSceneNR = 1;
            else currentSceneNR = 0;

            SceneManager.LoadScene(currentSceneNR);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
