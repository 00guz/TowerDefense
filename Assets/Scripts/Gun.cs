using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int damage = 2;

    private Enemy target;
    [Header("Partikül Efektleri")]
    public GameObject trailEffectPrefab; // 🔹 YENİ: FX_BulletTrail prefab'ını buraya sürükle
    private ParticleSystem trailParticleSystem;
    public GameObject hitEffectPrefab;

    public void SetTarget(Enemy newTarget, int newDamage)
    {
        target = newTarget;
        damage = newDamage;
    }

    private void Start() 
    {
        // Trail efekti oluştur ve merminin çocuğu yap
        if (trailEffectPrefab != null)
        {
            // 1. Efekti 'transform' (merminin) alt objesi olarak oluştur
            GameObject effectObject = Instantiate(trailEffectPrefab, transform);
            
            // 2. 🔹 KONUM AYARI (Kuyruk için) 🔹
            // 'Projectiles.png' resminize göre okunuz SAĞ-YUKARI (+X, +Y) bakıyor.
            // Bu yüzden kuyruğu SOL-AŞAĞI (-X, -Y) yönündedir.
            // Bu (-0.3f, -0.3f) değerlerini okun boyuna göre kendiniz ayarlayabilirsiniz.
            effectObject.transform.localPosition = new Vector3(0,0, 0); 

            // 3. 🔹 ROTASYON AYARI (Sizin bulduğunuz çözüm) 🔹
            // Resimde gösterdiğiniz (45, -90, -90) rotasyonunu C# koduyla atıyoruz.
            effectObject.transform.localRotation = Quaternion.Euler(45f, -90f, -90f);

            // 4. Parçacık sistemini bul ve oynat
            trailParticleSystem = effectObject.GetComponent<ParticleSystem>();
            if (trailParticleSystem != null)
            {
                trailParticleSystem.Play(); 
            }
        }
    }

    private void OnDestroy() // 🔹 YENİ: Mermi yok olduğunda trail'i durdur ve yok et
    {
        if (trailParticleSystem != null)
        {
            // Parent'lıktan çıkar ki mermi ile birlikte yok olmasın
            trailParticleSystem.transform.SetParent(null); 
            // Loop'u durdur ve kalan parçacıkların kaybolmasını bekle
            trailParticleSystem.Stop(); 
            // Belli bir süre sonra yok et (kalan parçacıklar kaybolduktan sonra)
            Destroy(trailParticleSystem.gameObject, trailParticleSystem.main.startLifetime.constantMax);
        }
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // --- 🔹 YENİ ROTASYON KODU (Sol-Aşağı Sprite için) 🔹 ---
        
        // 1. Hedefe olan yön vektörünü hesapla
        Vector3 direction = (target.transform.position - transform.position).normalized;

        // 2. Yön vektörünün açısını Z-ekseni için hesapla (Atan2)
        // Bu, hedefin "sağ" (0 derece) eksene göre açısını verir
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 3. Sprite Ofsetini Ayarla
        // Sizin sprite'ınız "sol-aşağı" (-135 derece) bakıyor.
        // Kodun 'angle' hesaplaması ise "sağ" (0 derece) eksenine göre yapıldı.
        // Sprite'ımızı "sağ" eksene hizalamak için +135 derecelik bir ofset eklemeliyiz.
        float angleOffset = 135f; 

        // 4. Merminin son rotasyonunu ayarla
        transform.rotation = Quaternion.Euler(0f, 0f, angle + angleOffset);

        // --- 🔹 KOD SONU 🔹 ---


        // Mermiyi hedefe doğru hareket ettir
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        // Hedefe ulaşıldı mı diye kontrol et
        if (Vector2.Distance(transform.position, target.transform.position) < 0.2f)
        {
            target.TakeDamage(damage);
            
            // 🔹 YENİ: Vurulma efekti oluştur ve oynat
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 1f); // 1 saniye sonra yok et
            }

            Destroy(gameObject);
        }
    }
}