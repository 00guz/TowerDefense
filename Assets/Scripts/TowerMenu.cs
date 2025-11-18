using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TowerMenu : MonoBehaviour
{
    public Transform buttonParent; // Butonların oluşturulacağı alan
    public GameObject towerButtonPrefab; // Buton prefabı (prefab içinde Button + Image + TMP olmalı)

    private TowerManager placementManager;
    public static TowerMenu Instance;

    [Header("Referanslar")]
    public TowerInfoPanel towerInfoPanel;
    private bool isMenuLocked = false; // Menü kilit durumu

    [Header("Panel Kaydırma Ayarları")]
    [Tooltip("Kaydırarak açıp kapatacağımız panel (Hiyerarşiden sürükleyin)")]
    public RectTransform slidablePanel; // 🔹 Kontrol edilecek panelin RectTransform'u

    [Tooltip("Bu paneli açıp kapatacak olan buton")]
    public Button toggleButton;
    public RectTransform toggleButtonImage;

    [Tooltip("Animasyonun ne kadar hızlı olacağı (saniye)")]
    public float slideDuration = 0.4f;

    // panelRect yerine slidablePanel kullanacağız
    private Vector2 openPosition;     
    private Vector2 closedPosition;
    private bool isPanelOpen = false;

    private void Awake() => Instance = this;

    private void Start()
    {
        placementManager = FindObjectOfType<TowerManager>();
        ShowAllowedTowers();

        if (slidablePanel == null)
        {
            Debug.LogError("Kaydırılacak panel (Slidable Panel) atanmamış!", this.gameObject);
            return;
        }

        // 1. Panelin başlangıç pozisyonunu "KAPALI" pozisyon olarak kaydet.
        closedPosition = slidablePanel.anchoredPosition;

        // 2. "AÇIK" pozisyonu hesapla:
        openPosition = new Vector2(closedPosition.x - slidablePanel.rect.width, closedPosition.y);

        // 3. Panelin başlangıçta kapalı olduğunu belirt
        isPanelOpen = false;

        // 4. Toggle butonuna basıldığında 'TogglePanel' metodunu çağır
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePanel);
        }
    }
    
    private void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        Vector2 targetPosition = isPanelOpen ? openPosition : closedPosition;

        // 🔹 'panelRect' yerine 'slidablePanel' kullanıyoruz
        float targetImageRotationY = isPanelOpen ? 180f : 0f;

        // 3. Panelin kayma animasyonunu başlat
        slidablePanel.DOAnchorPos(targetPosition, slideDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => {
                // 4. 🔹 ANİMASYON BİTİNCE: Resmin rotasyonunu ANINDA ayarla
                if (toggleButtonImage != null)
                {
                    // localEulerAngles kullanarak Z ve X'i koruyup sadece Y'yi değiştir
                    toggleButtonImage.localEulerAngles = new Vector3(
                        toggleButtonImage.localEulerAngles.x, 
                        targetImageRotationY, 
                        toggleButtonImage.localEulerAngles.z
                    );
                }
            });
    }
    public void SetMenuLock(bool isLocked)
    {
        isMenuLocked = isLocked;
        // Kilit durumu değiştiğinde, tüm butonların durumunu
        // bu yeni kilit durumuna göre yeniden güncelle.
        UpdateButtonInteractables();
    }

    public void ShowAllowedTowers()
    {
        // Eski butonları temizle
        foreach (Transform child in buttonParent)
            Destroy(child.gameObject);

        LevelData currentLevel = GameManager.Instance.levelDatabase.allLevels[GameManager.Instance.currentLevelIndex];

        if (currentLevel == null || currentLevel.allowedTowers == null || currentLevel.allowedTowers.Length == 0)
        {
            Debug.LogWarning("Bu levelde kullanılabilir kule yok!");
            return;
        }

        foreach (var tower in currentLevel.allowedTowers)
        {
            // local copy to avoid closure issues
            TowerData towerCopy = tower;

            GameObject buttonObj = Instantiate(towerButtonPrefab, buttonParent);
            Button button = buttonObj.GetComponentInChildren<Button>();
            Image icon = buttonObj.GetComponentInChildren<Image>();
            TextMeshProUGUI sorutext = buttonObj.transform.Find("SoruPanel").GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI costText = buttonObj.transform.Find("CostPanel").GetComponentInChildren<TextMeshProUGUI>();
            

            // attach or set TowerButton helper
            TowerButton tb = buttonObj.GetComponentInChildren<TowerButton>();
            if (tb == null)
                tb = buttonObj.AddComponent<TowerButton>();

            tb.towerData = towerCopy;
            tb.iconImage = icon;
            tb.button = button;


            // 🔹 YENİ KOD: InfoButton'u bul (Prefab'da adını "InfoButton" koyduğunuzu varsayıyoruz)
            Button infoButton = buttonObj.transform.Find("InfoButton").GetComponent<Button>();

            if (infoButton != null && ThemeManager.Instance != null)
            {
                Image infoButtonImage = infoButton.GetComponent<Image>();
                if (infoButtonImage != null)
                {
                    // ThemeManager'dan o anki doğru sprite'ı iste
                    infoButtonImage.sprite = ThemeManager.Instance.GetCurrentInfoButtonSprite();
                }
            }

            if (sorutext != null)
                sorutext.text = towerCopy.soruSayisi.ToString() + " Soru";
            if (costText != null)
                costText.text = towerCopy.cost.ToString()+ " Altın"; 

            if (icon != null)
                icon.sprite = towerCopy.icon;

            // Altına göre buton aktif/pasif ve renk
            bool canBuy = GameManager.Instance.altin >= towerCopy.cost;
            // Butonun tıklanabilir olması için:
            // 1. Menü kilitli OLMAMALI
            // 2. Oyuncunun parası YETMELİ
            bool finalInteractable = !isMenuLocked && canBuy;
            if (button != null)
                button.interactable = finalInteractable;

            if (icon != null)
                icon.color = finalInteractable ? Color.white : new Color(1f, 1f, 1f, 0.5f);

            // Listener ekle (kapanma sorununu önlemek için yerel değişkenle)
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => placementManager.SelectTower(towerCopy));
            }

            if (infoButton != null && towerInfoPanel != null)
            {
                infoButton.onClick.RemoveAllListeners();
                
                // Bu butona tıklandığında birden fazla işlem yapacağız:
                infoButton.onClick.AddListener(() => {
                
                    // 1. Bilgi panelini göster (eski kod)
                    towerInfoPanel.ShowInfo(towerCopy);
                    
                    // 2. YENİ KOD: Toggle butonunu devre dışı bırak
                    if (toggleButton != null)
                    {
                        toggleButton.interactable = false;
                    }
                });
            }
        }
    }

    public void UpdateButtonInteractables()
    {
        foreach (Transform child in buttonParent)
        {
            Button button = child.GetComponentInChildren<Button>();
            Image icon = child.GetComponentInChildren<Image>();
            TowerButton towerButton = child.GetComponentInChildren<TowerButton>();

            if (button == null || towerButton == null || towerButton.towerData == null) 
                continue;

            TowerData tower = towerButton.towerData;

            bool canBuy = GameManager.Instance.altin >= tower.cost;
            bool finalInteractable = !isMenuLocked && canBuy;

            button.interactable = finalInteractable;

            if (icon != null)
                icon.color = finalInteractable ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        }
    }
}
