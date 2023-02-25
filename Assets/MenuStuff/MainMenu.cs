using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    void Awake(){
        optionsMenu.SetActive(false);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OpenOptionsMenu()
    {
        gameObject.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
