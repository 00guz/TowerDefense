using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject levels;
    public GameObject settings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LevelsOpen()
    {
        levels.SetActive(true);
    }

    public void settingsOpen()
    {
        settings.SetActive(true);
    }

    public void settingsClose()
    {
        settings.SetActive(false);
    }
}
