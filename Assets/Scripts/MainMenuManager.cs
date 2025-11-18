using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public GameObject levels;
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
}
