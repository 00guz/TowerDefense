using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float currentHealth;
    private bool isDead = false;

    public delegate void EnemyDied(Enemy enemy);
    public event EnemyDied OnEnemyDied;
    [Header("Sağlık Barı")]
    [SerializeField] private GameObject healthBarCanvasObject; 
    
    // 🔹 YENİ: Slider'ı kod içinde saklamak için
    private Slider healthBarSlider;

    // 🔹 YENİ: Sprite'ı çevirmek için referans
    private SpriteRenderer spriteRenderer; 

    private void Start()
    {
        currentHealth = data.maxHealth;
        
        // 🔹 YENİ: Sağlık barını 100% dolu başlat
        if (healthBarCanvasObject != null)
        {
            // Canvas'ın altındaki Slider bileşenini bul
            healthBarSlider = healthBarCanvasObject.GetComponentInChildren<Slider>();
            
            if (healthBarSlider != null)
                healthBarSlider.value = 1f; // Değeri 1 yap
            
            // 🔹 EN ÖNEMLİ KISIM: BAŞLANGIÇTA GİZLE
            healthBarCanvasObject.SetActive(false); 
        }
        
        // 🔹 YENİ: SpriteRenderer bileşenini bul ve ata
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Eğer sprite ana objede değil de bir alt objedeyse (örn: "Gfx" objesi)
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
        if(spriteRenderer == null)
            Debug.LogWarning("Bu düşmanda SpriteRenderer bulunamadı!", gameObject);
    }

    public void SetPath(Transform[] points)
    {
        waypoints = points;
        transform.position = waypoints[0].position;
    }

    private void Update()
    {
        // Bu kontrolleriniz gayet iyi, kalmalı.
        if (isDead || waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];

        // --- 'target' null kontrolü (Sizdeki kod) ---
        if (target == null)
        {
            Debug.LogError($"Düşman {gameObject.name} için {currentWaypointIndex} index'li waypoint YOK EDİLMİŞ! Düşman yok ediliyor.");
            Die(false); 
            return;     
        }
        // --- Kontrol sonu ---


        // 🔹 YENİ: YÖN ÇEVİRME MANTIĞI 🔹
        if (spriteRenderer != null)
        {
            // Hedefin x konumu, şu anki konumumuzdan büyük mü?
            // (Küçük bir 'ölü bölge' (0.01f) ekleyerek, tam dikey giderken
            // sürekli yön değiştirmesini (jitter) engelleriz)
            
            if (target.position.x > transform.position.x + 0.01f)
            {
                // SAĞA GİDİYOR: Sprite'ı ters çevir ("tam tersi")
                spriteRenderer.flipX = true;
            }
            else if (target.position.x < transform.position.x - 0.01f)
            {
                // SOLA GİDİYOR: Sprite'ı normale çevir ("rotasyon 0")
                spriteRenderer.flipX = false;
            }
            // Eğer fark -0.01 ile 0.01 arasındaysa (yani dikey gidiyorsa)
            // mevcut yönünü koru, hiçbir şey yapma.
        }
        // 🔹 YÖN MANTIĞI SONU 🔹


        // Artık 'target.position' erişimi güvenli
        transform.position = Vector3.MoveTowards(transform.position, target.position, data.speed * Time.deltaTime);

        // Hata veren satır burasıydı, artık 'target' null olamayacağı için güvende.
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                ReachBase();
                return;
            }
        }
    }

    private void ReachBase()
    {
        if (isDead) return; 
        isDead = true;

        GameManager.Instance.TakeDamageBase(data.damageToBase);
        OnEnemyDied?.Invoke(this);

        // 🔹 YENİ: Ölünce sağlık barını da gizle/yok et
        if (healthBarCanvasObject != null)
            healthBarCanvasObject.SetActive(false); 
            
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; 

        currentHealth -= damage;
        
        // 🔹 YENİ: Sağlık barını güncelle
        if (healthBarCanvasObject != null && healthBarSlider != null)
        {
            // 1. Eğer gizliyse, ilk hasarda görünür yap
            if (!healthBarCanvasObject.activeSelf)
            {
                healthBarCanvasObject.SetActive(true);
            }

            // 2. Değeri güncelle
            float healthPercentage = currentHealth / (float)data.maxHealth;
            healthBarSlider.value = healthPercentage;
        }

        if (currentHealth <= 0)
            Die(true);
    }

    private void Die(bool killedByPlayer)
    {
        if (isDead) return; // 🔹 ikinci çağrıyı engelle
        isDead = true;

        if (killedByPlayer)
        {
            GameManager.Instance.AddSkor(data.reward);
            TowerMenu.Instance.UpdateButtonInteractables();
        }
        OnEnemyDied?.Invoke(this);

        if (healthBarCanvasObject != null)
            healthBarCanvasObject.SetActive(false);
            
        Destroy(gameObject);
    }
}
