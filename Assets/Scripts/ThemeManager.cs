using UnityEngine;
using UnityEngine.UI; // 🔹 UI (Image) bileşenlerini kullanabilmek için bu satır gerekli

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance;

    // 0 = İlkbahar (Level 1-8)
    // 1 = Kış (Level 9-16)
    // 2 = Çöl (Level 17-24)
    // 3 = Lav (Level 25-32)
    public int CurrentThemeIndex { get; private set; } = 0;

    // Her temada kaç level olduğunu belirtir
    private const int LEVELS_PER_THEME = 8;

    [Header("Temaya Göre Değişecek UI Panelleri")]
    [Tooltip("Değişmesini istediğiniz 7 adet UI Image panelini buraya sürükleyin")]
    public Image[] panelsToTheme; // 🔹 7 adet panelinizi buraya atacaksınız

    [Header("Tema Sprite Kümeleri")]
    [Tooltip("Her bir panel için 4 adet temalı sprite (0=İlkbahar, 1=Kış, 2=Çöl, 3=Lav)")]
    // 🔹 7 paneliniz için 7 ayrı sprite dizisi
    public Sprite[] panel_1_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_2_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_3_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_4_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_5_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_6_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_7_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_8_ThemeSprites; // (Inspector'dan 4 sprite atayın)
    public Sprite[] panel_9_ThemeSprites;
    public Sprite[] panel_10_ThemeSprites;
    public Sprite[] panel_11_ThemeSprites;
    public Sprite[] panel_12_ThemeSprites;
    public Sprite[] panel_13_ThemeSprites;
    public Sprite[] panel_14_ThemeSprites;
    public Sprite[] panel_15_ThemeSprites;
    public Sprite[] panel_16_ThemeSprites;
    public Sprite[] panel_17_ThemeSprites;
    public Sprite[] panel_18_ThemeSprites;
    public Sprite[] panel_19_ThemeSprites;
    public Sprite[] panel_20_ThemeSprites;
    public Sprite[] panel_21_ThemeSprites;
    public Sprite[] panel_22_ThemeSprites;
    public Sprite[] panel_23_ThemeSprites;
    public Sprite[] panel_24_ThemeSprites;
    public Sprite[] panel_25_ThemeSprites;
    public Sprite[] infoButtonThemeSprites; // 🔹 Info butonu için temalı sprite'lar



    private Sprite[][] allThemeSprites; // 🔹 Sprite'ları kolay erişim için birleştireceğiz


    private void Awake()
    {
        // Basit bir Singleton (Tekil Yönetici) deseni
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        // 🔹 Sprite dizilerini tek bir büyük dizide topla (daha kolay erişim için)
        allThemeSprites = new Sprite[][]
        {
            panel_1_ThemeSprites,
            panel_2_ThemeSprites,
            panel_3_ThemeSprites,
            panel_4_ThemeSprites,
            panel_5_ThemeSprites,
            panel_6_ThemeSprites,
            panel_7_ThemeSprites,
            panel_8_ThemeSprites,
            panel_9_ThemeSprites,
            panel_10_ThemeSprites,
            panel_11_ThemeSprites,
            panel_12_ThemeSprites,
            panel_13_ThemeSprites,
            panel_14_ThemeSprites,
            panel_15_ThemeSprites,
            panel_16_ThemeSprites,
            panel_17_ThemeSprites,
            panel_18_ThemeSprites,
            panel_19_ThemeSprites,
            panel_20_ThemeSprites,
            panel_21_ThemeSprites,
            panel_22_ThemeSprites,
            panel_23_ThemeSprites,
            panel_24_ThemeSprites,
            panel_25_ThemeSprites
        };
    }

    // Bu fonksiyon, GameManager tarafından oyun başlarken çağrılacak
    public void SetThemeFromLevel(int currentLevelIndex)
    {
        // Hangi temada olduğumuzu basit bir matematik ile hesapla
        CurrentThemeIndex = currentLevelIndex / LEVELS_PER_THEME;
        
        Debug.Log($"Tema ayarlandı! Level: {currentLevelIndex + 1}, Tema Index: {CurrentThemeIndex}");
    }

    // Bu fonksiyon, temayı ayarladıktan HEMEN SONRA çağrılacak
    public void ApplyThemeToPanels()
    {
        if (panelsToTheme == null || panelsToTheme.Length == 0)
        {
            Debug.LogWarning("ThemeManager: Değiştirilecek panel atanmamış.");
            return;
        }

        // 'panelsToTheme' dizisindeki her bir paneli (Image) döngüye al
        for (int i = 0; i < panelsToTheme.Length; i++)
        {
            // Eğer o panel (Image) atanmışsa...
            if (panelsToTheme[i] != null)
            {
                // O panele karşılık gelen sprite kümesini al (örn: panel_1_ThemeSprites)
                Sprite[] currentSpriteGroup = allThemeSprites[i];

                // Sprite kümesinin geçerli olup olmadığını kontrol et
                if (currentSpriteGroup != null && currentSpriteGroup.Length > CurrentThemeIndex)
                {
                    // 🔹 İŞTE BURASI: Panelin sprite'ını o anki temaya göre ayarla!
                    panelsToTheme[i].sprite = currentSpriteGroup[CurrentThemeIndex];
                }
                else
                {
                    Debug.LogWarning($"Panel {i} için Tema {CurrentThemeIndex} sprite'ı eksik!");
                }
            }
        }
    }
    public Sprite GetCurrentInfoButtonSprite()
    {
        // 'CurrentThemeIndex' (0, 1, 2, veya 3) zaten hesaplanmış olmalı
        
        // Eğer o tema için bir sprite atanmışsa onu döndür
        if (infoButtonThemeSprites != null && infoButtonThemeSprites.Length > CurrentThemeIndex)
        {
            return infoButtonThemeSprites[CurrentThemeIndex];
        }

        // Eğer atanmamışsa, bir uyarı ver ve güvenlik için varsayılanı (ilkbahar) döndür
        Debug.LogWarning($"InfoButton için Tema {CurrentThemeIndex} sprite'ı eksik! Varsayılan kullanılıyor.");
        if (infoButtonThemeSprites != null && infoButtonThemeSprites.Length > 0)
        {
            return infoButtonThemeSprites[0];
        }

        return null; // Hiç sprite atanmamışsa
    }
    
}