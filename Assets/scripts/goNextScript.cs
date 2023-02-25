using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goNextScript : MonoBehaviour
{
    [SerializeField]AudioClip musicClip;
    void Start()
    {
        AudioManager.Instance.PlayMusic(musicClip);
    }
    public void GoNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public void quitgame()
    {
        Application.Quit();
    }
}
