using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetButtonDown("ButtonRight"))
        {
            LoadSceneOne();
        }
    }

    public void LoadSceneOne()
    {
        SceneManager.LoadScene(1);
    }
}
