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
        UIBubbleTween bubble = levels.GetComponent<UIBubbleTween>();
        if (bubble != null) 
            bubble.Open();
        else 
            levels.SetActive(true);
    }

    public void LevelsClose()
    {
        UIBubbleTween bubble = levels.GetComponent<UIBubbleTween>();
        if (bubble != null) 
            bubble.Close();
        else 
            levels.SetActive(false);
    }

    public void settingsOpen()
    {
        UIBubbleTween bubble = settings.GetComponent<UIBubbleTween>();
        if (bubble != null) 
            bubble.Open();
        else 
            settings.SetActive(true);
    }

    public void settingsClose()
    {
        UIBubbleTween bubble = settings.GetComponent<UIBubbleTween>();
        if (bubble != null) 
            bubble.Close();
        else 
            settings.SetActive(false);
    }
}
