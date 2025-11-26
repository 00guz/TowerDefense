using UnityEngine;
using System.Linq;

public class Tower : MonoBehaviour
{
    public TowerData data;
    public GameObject Gun; 
    private float fireCooldown;

    [Header("Partikül Efektleri")]
    public GameObject shotEffectPrefab;

    // -------------------------------------------------------------------
    // 🔹 YENİ EKLENEN DEĞİŞKENLER 🔹
    [Header("Namlunun Konum Ayarları")]
    [Tooltip("Merminin, kulenin merkezinden Y ekseninde ne kadar YUKARIDA spawn olacağı.")]
    [SerializeField] private float spawnNamlusuYuksekligi = 0.2f;

    [Tooltip("Mermi sağa/sola giderken, X ekseninde ne kadar YANA ötelenerek spawn olacağı.")]
    [SerializeField] private float spawnYanalOteleme = 0.2f;
    // -------------------------------------------------------------------


    private void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            // Sahnedeki tüm düşmanları bul
            var enemies = FindObjectsOfType<Enemy>();

            // 🔹 YENİ HEDEFLEME MANTIĞI 🔹
            var target = enemies
                // 1. Önce sadece MENZİL İÇİNDEKİLERİ filtrele
                .Where(e => Vector2.Distance(transform.position, e.transform.position) <= data.range)
                
                // 2. Sonra CAN DEĞERİNE göre sırala (Küçükten büyüğe)
                // GetCurrentHealth() fonksiyonunu kullanıyoruz
                .OrderBy(e => e.GetCurrentHealth()) 
                
                // 3. (İsteğe Bağlı) Canları eşitse en yakındakini seç (İkincil kural)
                .ThenBy(e => Vector2.Distance(transform.position, e.transform.position))

                // 4. İlk sıradakini (yani en az canlıyı) al
                .FirstOrDefault();

            if (target != null)
            {
                Shoot(target);
                fireCooldown = 1f / data.fireRate;
            }
        }
    }

    // -------------------------------------------------------------------
    // 🔹 GÜNCELLENMİŞ SHOOT METODU 🔹
    private void Shoot(Enemy target)
    {
        // 1. YÖN ve ROTASYON (Merminin hedefe bakması için)
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

        
        // 2. DİNAMİK X HESAPLAMASI
        // direction.x'in işaretini alır (-1 Sola, +1 Sağa, 0 Merkeze)
        // Bunu 'spawnYanalOteleme' ile çarparak dinamik X pozisyonunu buluruz.
        float dinamikX = Mathf.Sign(direction.x) * spawnYanalOteleme;

        // 3. SABİT Y HESAPLAMASI
        // Y pozisyonu her zaman sizin girdiğiniz 'spawnNamlusuYuksekligi' değişkeni olacak.
        float sabitY = spawnNamlusuYuksekligi;


        // 4. BAŞLANGIÇ KONUMU
        // Kulenin pozisyonu + (dinamikX, sabitY, 0)
        Vector3 spawnPosition = transform.position + new Vector3(dinamikX, sabitY, 0f);


        // 5. Mermiyi ve Efekti bu yeni konumda ve rotasyonda oluştur
        GameObject proj = Instantiate(Gun, spawnPosition, rotation);
        proj.GetComponent<Gun>().SetTarget(target, data.damage);
        
        if (shotEffectPrefab != null)
        {
            GameObject effect = Instantiate(shotEffectPrefab, spawnPosition, rotation);
            Destroy(effect, 1f);
        }

        if (AudioManager.Instance != null && data.shootSoundIndex >= 0)
        {
            // Örneğin data.shootSoundName "OkSesi" ise onu çalar
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxClip[data.shootSoundIndex]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (data != null)
        {
            Gizmos.color = Color.red;
            // Kulenin etrafına gerçek menzili (data.range) çizer
            Gizmos.DrawWireSphere(transform.position, data.range);
        }
    }
    // -------------------------------------------------------------------
}