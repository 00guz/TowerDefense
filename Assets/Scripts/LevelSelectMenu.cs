using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Veritabanı")]
    public AllLevelsData levelDatabase; // "LevelDB" asset'ini buraya sürükleyin

    [Header("UI Elemanları")]
    public GameObject levelButtonPrefab; // Level buton prefabı
    public Transform levelButtonContainer; // Grid Layout Group olan obje

    // -------------------------------------------------------------------
    // 🔹 YENİ EKLENEN DEĞİŞKENLER 🔹
    [Header("Level Buton Görselleri")]
    [Tooltip("Her 8 levelde bir deÄŸiÅŸecek olan buton sprite'larÄ±. (SÄ±rayla 4 adet atayÄ±n)")]
    public Sprite[] levelButtonSprites;
    // -------------------------------------------------------------------

    [Header("Sayfalama Ayarları (Pagination)")]
    public int itemsPerPage = 12; // 🔹 Her sayfada kaç level butonu olacak
    public Button nextPageButton; // 🔹 "Sağa Git" butonu
    public Button prevPageButton; // 🔹 "Sola Git" butonu
    public TextMeshProUGUI pageNumberText; // 🔹 (Opsiyonel) "1 / 4" yazan metin

    [Header("Sahne Ayarları")]
    public string gameSceneName = "GameScene"; // Oyun sahnesinin adı

    private int currentPage = 0;
    private int totalPages = 0;
    [Header("Geliştirici Ayarları")]
    [Tooltip("İşaretlenirse menüdeki tüm leveller açık görünür.")]
    public bool debugUnlockAllLevels = false;

    void Start()
    {
        // 1. Toplam sayfa sayısını hesapla
        if (levelDatabase.allLevels.Length == 0) return;
        
        totalPages = (levelDatabase.allLevels.Length + itemsPerPage - 1) / itemsPerPage;

        // 2. Ok butonlarına dinleyicileri (listener) ekle
        nextPageButton.onClick.AddListener(NextPage);
        prevPageButton.onClick.AddListener(PreviousPage);

        // 3. İlk sayfayı (sayfa 0) göster
        ShowPage(0);
    }

    // 🔹 "Sola Git" butonu
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
    }

    // 🔹 "Sağa Git" butonu
    public void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            ShowPage(currentPage + 1);
        }
    }

    // 🔹 Belirtilen sayfadaki level'ları oluşturan ana fonksiyon
    void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;

        // 1. Önceki sayfadan kalan tüm eski butonları temizle
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Kayıtlı en yüksek level'ı PlayerPrefs'ten al
        int highestLevelUnlocked = PlayerPrefs.GetInt("SavedLevelIndex", 0);

        // Eğer bu script üzerindeki kutucuk işaretliyse, hepsini aç
        if (debugUnlockAllLevels)
        {
            highestLevelUnlocked = 99999; 
        }

        // 3. Bu sayfada gösterilecek levelların başlangıç ve bitiş index'ini hesapla
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, levelDatabase.allLevels.Length);

        // 4. Sadece bu aralıktaki levellar için buton oluştur
        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI levelText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Transform lockIcon = buttonObj.transform.Find("LockIcon"); 

            // -------------------------------------------------------------------
            // 🔹 YENİ: BUTON SPRITE'INI AYARLA (Önceki kodunuz)
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null && levelButtonSprites != null && levelButtonSprites.Length > 0)
            {
                int spriteIndex = i / 8;
                if (spriteIndex < levelButtonSprites.Length)
                    buttonImage.sprite = levelButtonSprites[spriteIndex];
                else
                    buttonImage.sprite = levelButtonSprites[levelButtonSprites.Length - 1];
            }
            // -------------------------------------------------------------------

            // Level numarasını yaz
            levelText.text = (i + 1).ToString();

            // -------------------------------------------------------------------
            // 🔹 YENİ: TEXT RENGİNİ TEMAYA GÖRE AYARLA 🔹
            if (ThemeManager.Instance != null && ThemeManager.Instance.themeTextColors != null)
            {
                // Bu levelin hangi temaya (0, 1, 2, 3) ait olduğunu hesapla
                int themeIndexForThisLevel = i / 8;

                // Eğer renk dizisinde bu index varsa rengi ata
                if (themeIndexForThisLevel < ThemeManager.Instance.themeTextColors.Length)
                {
                    levelText.color = ThemeManager.Instance.themeTextColors[themeIndexForThisLevel];
                }
                else
                {
                    // Eğer level sayısı tema sayısından fazlaysa son rengi kullan
                    levelText.color = ThemeManager.Instance.themeTextColors[ThemeManager.Instance.themeTextColors.Length - 1];
                }
            }
            // -------------------------------------------------------------------

            bool isUnlocked = (i <= highestLevelUnlocked);

            if (isUnlocked)
            {
                if (lockIcon != null) lockIcon.gameObject.SetActive(false);
                button.interactable = true;
                int levelIndexToLoad = i; 
                button.onClick.AddListener(() => StartLevel(levelIndexToLoad));
            }
            else
            {
                if (lockIcon != null) lockIcon.gameObject.SetActive(true);
                button.interactable = false;
            }
        }
        
        // 5. Ok butonlarını ve sayfa numarasını güncelle
        UpdatePaginationButtons();
    }

    // 🔹 Ok butonlarının durumunu (tıklanabilir/tıklanamaz) ayarlar
    void UpdatePaginationButtons()
    {
        // Eğer ilk sayfadaysak (sayfa 0) "Sol Ok" butonunu kapat
        prevPageButton.interactable = (currentPage > 0);

        // Eğer son sayfadaysak "Sağ Ok" butonunu kapat
        nextPageButton.interactable = (currentPage < totalPages - 1);

        // (Opsiyonel) Sayfa metnini güncelle
        if (pageNumberText != null)
        {
            pageNumberText.text = $"{currentPage + 1} / {totalPages}";
        }
    }

    // 🔹 Bu fonksiyon DOKUNULMADAN KALDI (Aynı, çalışıyor)
    public void StartLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("SelectedLevelToLoad", levelIndex);
        PlayerPrefs.SetInt("IsLoadingFromMenu", 1); // 1 = true
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }
}