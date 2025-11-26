using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask placeableLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private SpriteRenderer ghostRenderer;
    [Header("Grid References")] // 🔹 YENİ BAŞLIK
    [SerializeField] private Tilemap placeableTilemap; // 🔹 YENİ: Buraya "kahverengi karelerin" olduğu Tilemap'i sürükleyin

    [Header("Range Indicator")]
    [SerializeField] private Transform rangeIndicator; // Ghost tower etrafındaki alan objesi (Circle veya Sprite)

    [Header("Ghost Colors")]
    [SerializeField] private Color validColor = new(0f, 1f, 0f, 0.5f);
    [SerializeField] private Color invalidColor = new(1f, 0f, 0f, 0.5f);

    private TowerData selectedTower;
    private bool canPlaceHere = false;
    public QuestionManager questionManager;

    public static TowerManager Instance;

    // 🔹 YENİ Awake METODU 
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 🔹 BURADAKİ ARAMA KODUNU SİLİN VEYA YORUM SATIRI YAPIN
        // FindPlaceableTilemapInScene(); 
        
        // Veya daha iyisi, bir "fallback" (yedek) olarak kalsın,
        // ama asıl komut GameManager'dan gelecek.
        if (placeableTilemap == null)
            FindPlaceableTilemapInScene();
    }

    public void FindPlaceableTilemapInScene()
    {
        // 1. Sahnedeki TÜM Tilemap bileşenlerini bul
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();

        if (allTilemaps.Length == 0)
        {
            Debug.LogError("Sahne'de HİÇ Tilemap bulunamadı!");
            return;
        }

        // 2. Bu Tilemap'leri döngüye al
        foreach (Tilemap map in allTilemaps)
        {
            // 3. Bu tilemap'in katmanını (layer),
            //    bizim 'placeableLayer' maskemizin içerip içermediğini kontrol et.
            int mapLayer = map.gameObject.layer;
            
            if ((placeableLayer.value & (1 << mapLayer)) != 0)
            {
                // 4. Eşleşme bulundu! Bu map'i ata ve aramayı durdur.
                placeableTilemap = map;
                Debug.Log($"✅ Yerleştirilebilir Tilemap otomatik bulundu: {map.gameObject.name}");
                return;
            }
        }

        // 5. Döngü biter ve hala bulunamazsa
        if (placeableTilemap == null)
        {
            Debug.LogError("DİKKAT! Sahnedeki hiçbir Tilemap, TowerManager'daki 'Placeable Layer' maskesiyle eşleşmiyor!");
        }
    }

    private void Update()
    {
        if (selectedTower == null)
        {
            ghostRenderer.enabled = false;
            if (rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);
            return;
        }

        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0) && canPlaceHere)
            PlaceTower();
    }

    private void UpdateGhostPosition()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, placeableLayer);

        if (hit.collider != null && placeableTilemap != null)
        {
            // 1. Farenin değdiği 1x1'lik hücreyi bul
            Vector3 worldPos = hit.point;
            Vector3Int cellPosition = placeableTilemap.WorldToCell(worldPos);

            // 2. O hücrenin ait olduğu 2x2'lik grubun sol-alt hücresini bul
            // Örn: Tıkladığımız [0,0], [1,0], [0,1], veya [1,1] ise, 'base' hep [0,0] olur.
            int groupBaseX = Mathf.FloorToInt(cellPosition.x / 2f) * 2;
            int groupBaseY = Mathf.FloorToInt(cellPosition.y / 2f) * 2;

            // 3. 🔹 YENİ VE KESİN MERKEZ HESAPLAMASI 🔹
            
            // 2x2'lik grubun başladığı sol-alt KÖŞEYİ al (örn: [0,0]'ın sol-alt köşesi)
            Vector3 bottomLeftCorner = placeableTilemap.CellToWorld(new Vector3Int(groupBaseX, groupBaseY, 0));
            
            // 2x2'lik grubun bittiği sağ-üst KÖŞEYİ al (örn: [1,1]'in sağ-üst köşesi, ki bu [2,2]'nin sol-alt köşesidir)
            Vector3 topRightCorner = placeableTilemap.CellToWorld(new Vector3Int(groupBaseX + 2, groupBaseY + 2, 0));

            // 4. Bu iki zıt köşenin tam ortalamasını al. 
            // Bu, 2x2'lik alanın %100 geometrik merkezidir.
            Vector3 trueCenter = (bottomLeftCorner + topRightCorner) / 2f;

            // 5. Ghost'u ve menzil göstergesini bu TAM MERKEZE ata
            ghostRenderer.transform.position = trueCenter;

            if (rangeIndicator != null)
            {
                rangeIndicator.position = trueCenter;
                rangeIndicator.gameObject.SetActive(true);
            }

            // 6. Engel kontrolünü bu merkezden yap
            // Yarıçapı 2x2'lik alanı kapsayacak şekilde ayarlayın (örn: 1.0f)
            Collider2D[] colliders = Physics2D.OverlapCircleAll(trueCenter, 1.0f, obstacleLayer);
            
            canPlaceHere = colliders.Length == 0; 
            
            // 7. Görünürlüğü ve rengi ayarla
            ghostRenderer.enabled = true; 
            ghostRenderer.color = canPlaceHere ? validColor : invalidColor;
        }
        else
        {
            // Fare 'placeableLayer' üzerinde değilse her şeyi kapat
            ghostRenderer.enabled = false;
            canPlaceHere = false;
            if (rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);
        }
    }

    private void PlaceTower()
    {
        Instantiate(selectedTower.prefab, ghostRenderer.transform.position, Quaternion.identity);
        Debug.Log($"✅ {selectedTower.towerName} yerleştirildi!");

        selectedTower = null;
        ghostRenderer.enabled = false;

        if (rangeIndicator != null)
            rangeIndicator.gameObject.SetActive(false);
    }

    // 🔹 UI butonları bu fonksiyonu çağıracak
    public void SelectTower(TowerData tower)
    {
        // Seçimi geçici olarak iptal et
        selectedTower = null;
        ghostRenderer.enabled = false;
        if (rangeIndicator != null) rangeIndicator.gameObject.SetActive(false);

        // Altın düş
        GameManager.Instance.altin -= tower.cost;
        GameManager.Instance.UpdateUI();
        
        // 🔹 BU SATIRI SİLİN (Artık SetMenuLock içinde yapılıyor)
        // TowerMenu.Instance.UpdateButtonInteractables();

        // 🔹 YENİ KOD: Soru sormadan hemen önce menüyü KİLİTLE
        TowerMenu.Instance.SetMenuLock(true);

        StartCoroutine(AskTowerQuestions(tower));
    }

    private IEnumerator AskTowerQuestions(TowerData tower)
    {
        int correctAnswers = 0;

        for (int i = 0; i < tower.soruSayisi; i++)
        {
            bool answered = false;

            questionManager.AskQuestion((bool correct) =>
            {
                if (correct){
                    correctAnswers++;
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxClip[10]);
                    questionManager.ShowResult(true, () => {
                            answered = true; 
                        });
                }

                else{
                    correctAnswers = -1; // yanlış cevap işareti
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxClip[11]);
                    questionManager.ShowResult(false, () => {
                            answered = true; 
                            });
                }
            });

            yield return new WaitUntil(() => answered);

            if (correctAnswers == -1)
            {
                Debug.Log("Yanlış cevap! Kule kullanılamaz.");
                // 🔹 YENİ KOD: Menü kilidini AÇ (Çıkış 1)
                TowerMenu.Instance.SetMenuLock(false);
                yield break;
            }
        }

        // 🔹 Tüm sorular doğruysa ghost tower aktif
        selectedTower = tower;
        ghostRenderer.sprite = tower.icon;
        ghostRenderer.enabled = true;

        if (rangeIndicator != null)
        {
            rangeIndicator.gameObject.SetActive(true);
            float diameter = (tower.range * 2f) / ghostRenderer.transform.localScale.x;
            rangeIndicator.localScale = new Vector3(diameter, diameter, 1f); // 2D için Z=1
        }

        Debug.Log($"🏰 {tower.towerName} seçildi ve artık yerleştirilebilir!");
        TowerMenu.Instance.SetMenuLock(false);
    }
}
