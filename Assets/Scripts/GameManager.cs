using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Altın ve Level Sistemi")]
    public int altin = 50;
    public TextMeshProUGUI altinText;

    public AllLevelsData levelDatabase;        // 🔹 Tüm level ayarları (ScriptableObject)
    public int currentLevelIndex = 0;    // 🔹 Şu anki level index
    public WaveSpawner waveSpawner;      // 🔹 Dalga yöneticisi
    public TextMeshProUGUI levelText;    // 🔹 "Level 1" yazısı
    public TextMeshProUGUI waveText;     // 🔹 "Dalga 1/5" yazısı

    [Header("Oyuncu Can Sistemi")]
    public int can = 10;                 // 🔹 Oyuncunun canı
    public TextMeshProUGUI canText;      // 🔹 Can UI’si
    public GameObject gameOverPanel;     // 🔹 Oyun bittiğinde açılan panel

    [Header("UI Panelleri")]
    public GameObject CongratsPanel;     // 🔹 Level tamamlandığında gösterilecek panel

    public GameObject AllLevelCompletedPanel; // Tüm level'lar tamamlandığında gösterilecek panel
    public GameObject pauseMenuPanel; // 🔹 Pause menüsü panel
    public GameObject settingsPanel; // 🔹 Settings menüsü panel

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // 🔹 PlayerPrefs'ten menüden mi geldiğimizi kontrol et
        if (PlayerPrefs.GetInt("IsLoadingFromMenu", 0) == 1)
        {
            // EVET, menüden geldik. Seçilen level'ı yükle.
            currentLevelIndex = PlayerPrefs.GetInt("SelectedLevelToLoad", 0);

            // Bu "bayrağı" sıfırla ki bir sonraki "RestartLevel" normal çalışsın
            PlayerPrefs.SetInt("IsLoadingFromMenu", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // HAYIR, normal "Devam Et" durumu. Kayıtlı ilerlemeyi yükle.
            currentLevelIndex = PlayerPrefs.GetInt("SavedLevelIndex", 0);
        }

        // 🔹 Eğer kaydedilen index, toplam level sayısından büyükse (örn. oyuna güncelleme gelip level sildiysek)
        // güvenli olması için index'i sıfırla.
        if (currentLevelIndex >= levelDatabase.allLevels.Length) // 'allLevels' -> 'levelDatabase.allLevels' oldu
        {
            currentLevelIndex = 0;
            PlayerPrefs.SetInt("SavedLevelIndex", 0);
            PlayerPrefs.SetInt("SelectedLevelToLoad", 0); // Bunu da temizleyelim
            PlayerPrefs.Save();
        }

        if (ThemeManager.Instance != null)
        {
            // 1. 🔹 ÖNCE: Hangi temada olduğumuzu ayarla
            ThemeManager.Instance.SetThemeFromLevel(currentLevelIndex);

            // 2. 🔹 SONRA: O temayı 7 panele uygula
            ThemeManager.Instance.ApplyThemeToPanels();
        }
        else
        {
            Debug.LogError("Sahne'de ThemeManager objesi bulunamadı!");
        }

        UpdateUI();
        StartLevel(currentLevelIndex);
    }
    

    // 🔹 Skor ekleme fonksiyonu (Enemy öldüğünde çağrılır)
    public void AddSkor(int amount)
    {
        altin += amount;
        UpdateUI();
    }

    // 🔹 Oyuncudan can düşür
    public void TakeDamageBase(int amount)
    {
        can -= amount;
        if (can < 0) can = 0;
        UpdateUI();

        if (can == 0)
        {
            GameOver();
        }
    }

    // 🔹 Oyun bittiğinde çağrılır
    private void GameOver()
    {
        Debug.Log("💀 Oyun Bitti!");
        Time.timeScale = 0f;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // 🔹 Arayüz güncelleme
    public void UpdateUI()
    {
        if (altinText != null)
            altinText.text = "Altın: " + altin;

        if (levelText != null && levelDatabase.allLevels.Length > 0)
            levelText.text = "Level: " + (currentLevelIndex + 1);

        if(waveText != null)
            waveText.text = "Dalga: " + waveSpawner.currentWaveIndex  + "/" + waveSpawner.totalWaves;

        if (canText != null)
            canText.text = "Can: " + can;
        
        if (ThemeManager.Instance != null)
        {
            // 1. 🔹 ÖNCE: Hangi temada olduğumuzu ayarla
            ThemeManager.Instance.SetThemeFromLevel(currentLevelIndex);

            // 2. 🔹 SONRA: O temayı 7 panele uygula
            ThemeManager.Instance.ApplyThemeToPanels();
        }
    }

    // 🔹 Level başlatma
    public GameObject currentMapInstance;

    // GameManager.cs içinde
    public void StartLevel(int index)
    {
        if (index < 0 || index >= levelDatabase.allLevels.Length)
        {
            Debug.LogError("Level index geçersiz!");
            return;
        }

        // 🔹 1. Eski haritayı bir değişkene kaydet
        GameObject oldMapInstance = currentMapInstance;

        // 🔹 2. Yeni level ayarlarını yap
        currentLevelIndex = index;
        LevelData level = levelDatabase.allLevels[index];
        altin = level.startingGold;
        Transform[] pathPoints = null;

        // 🔹 3. Yeni TileMap'i spawn et
        if (level.tileMapData != null && level.tileMapData.mapPrefab != null)
        {
            currentMapInstance = Instantiate(level.tileMapData.mapPrefab);

            if (TowerManager.Instance != null)
            {
                TowerManager.Instance.FindPlaceableTilemapInScene();
            }

            // ... (Path bulma kodunuzun tamamı burada...)
            if (level.tileMapData.enemyPathPoints != null && level.tileMapData.enemyPathPoints.Length > 0)
            {
                pathPoints = level.tileMapData.enemyPathPoints;
            }
            else
            {
                Transform pathRoot = currentMapInstance.transform.Find("Path");
                if (pathRoot != null)
                {
                    List<Transform> points = new List<Transform>();
                    foreach (Transform t in pathRoot)
                        points.Add(t);
                    pathPoints = points.ToArray();
                }
            }
            
            // 🔹 4. YENİ YÖNTEM: WaveSpawner'a HEM DATAYI HEM YOLU aynı anda ver
            if (pathPoints != null && pathPoints.Length > 0)
            {
                waveSpawner.PrepareLevel(level, pathPoints);
            }
            else
            {
                Debug.LogWarning("⚠ Path noktaları atanmadı!");
            }
        }

        // 🔹 5. ARTIK ESKİ HARİTAYI GÜVENLE SİLEBİLİRSİN
        if (oldMapInstance != null)
        {
            Destroy(oldMapInstance);
        }

        // 🔹 6. UI'yı güncelle
        UpdateUI();
        // 🔹 YENİ EKLENEN KOD: Kule menüsünü yeni level'a göre güncelle
        if (TowerMenu.Instance != null)
        {
            TowerMenu.Instance.ShowAllowedTowers();
        }
    }

        // 🔹 Düşman spawn pozisy





    // 🔹 Level tamamlandığında çağrılır
    public void LevelFinished()
    {
        Time.timeScale = 0f; // Oyunu durdur
        if(currentLevelIndex == levelDatabase.allLevels.Length-1)
        {
            Debug.Log("🎉 Tüm level'lar tamamlandı!");
            AllLevelCompletedPanel.SetActive(true);
            return;
        }
        CongratsPanel.SetActive(true);
    }

    // 🔹 Butondan çağrılır (Sonraki level)
    // 🔹 Butondan çağrılır (Sonraki level)
    public void NextLevel()
    {
        CongratsPanel.SetActive(false);
        Time.timeScale = 1f;

        currentLevelIndex++;
        Debug.Log($"➡ Geçiliyor: Level {currentLevelIndex + 1}");

        // 🔹 YENİ KOD: Yeni level index'ini kaydet
        PlayerPrefs.SetInt("SavedLevelIndex", currentLevelIndex);
        PlayerPrefs.Save(); // (Değişikliklerin diske yazılmasını garanti eder)

        LevelResetter.Instance.ResetLevel(); // Level sıfırlama
        StartLevel(currentLevelIndex);
    }

    public void RestartLevel()
    {
        gameOverPanel.SetActive(false);
        AllLevelCompletedPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        CongratsPanel.SetActive(false);
        
        // 1. Önce Kuleleri ve Canı sıfırla
        LevelResetter.Instance.ResetLevel();
        
        Time.timeScale = 1f;
        
        // 2. Sonra MEVCUT leveli yeniden başlat
        StartLevel(currentLevelIndex);
    }

    // 🔹 Ana menüye dön
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void settingsOpen()
    {
        settingsPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
    }
    public void settingsClose()
    {
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }

}
