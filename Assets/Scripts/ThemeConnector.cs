using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThemeConnector : MonoBehaviour
{
    [Header("Bu Sahnedeki Paneller (Sırasıyla)")]
    public Image[] localPanels;

    [Header("Bu Sahnedeki Textler")]
    public TextMeshProUGUI[] localTexts;

    private void Awake()
    {
        // Sahne başlar başlamaz (GameManager çalışmadan önce)
        // bu referansları ThemeManager'a aktaralım.
        
        if (ThemeManager.Instance != null)
        {
            // 1. Manager'ın listesini bu sahnedekilerle güncelle
            ThemeManager.Instance.panelsToTheme = localPanels;
            ThemeManager.Instance.textsToTheme = localTexts;

            // 2. (Opsiyonel) Hemen güncellemeyi tetikle
            // GameManager.Start() da bunu yapacak ama garanti olsun.
            int currentLevel = PlayerPrefs.GetInt("SavedLevelIndex", 0);
            // Eğer menüden seçilen level varsa onu kullan
            if (PlayerPrefs.GetInt("IsLoadingFromMenu", 0) == 1)
            {
                currentLevel = PlayerPrefs.GetInt("SelectedLevelToLoad", 0);
            }

            ThemeManager.Instance.SetThemeFromLevel(currentLevel);
            ThemeManager.Instance.ApplyThemeToPanels();
        }
    }
}