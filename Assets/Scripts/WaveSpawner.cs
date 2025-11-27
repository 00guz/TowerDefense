using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private LevelData levelData;

    private Transform[] enemyPathPoints;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool spawningFinished = false;

    public int currentWaveIndex = 0;
    public int totalWaves = 0;

    private Vector3 spawnPoint = Vector3.zero;



    public void SetSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }

    // 🔹 YENİ, BİRLEŞTİRİLMİŞ METOD 🔹
    public void PrepareLevel(LevelData newLevel, Transform[] newPath)
    {
        // 1. Önceki tüm işlemleri durdur
        StopAllCoroutines();
        
        // 2. Kalan eski düşmanları temizle
        ClearEnemies(); 

        // 3. Yeni bilgileri ata
        levelData = newLevel;
        enemyPathPoints = newPath;
        
        // 4. Bilgiler hazır olduğuna göre spawn işlemini güvenle başlat
        StartCoroutine(SpawnLevel());
    }


    private IEnumerator SpawnLevel()
    {
        spawningFinished = false;

        if (levelData == null)
        {
            Debug.LogWarning("⚠ LevelData atanmadı!");
            yield break;
        }

        if (levelData.waves == null || levelData.waves.Length == 0)
        {
            Debug.LogWarning("⚠ LevelData içinde hiç wave yok!");
            spawningFinished = true;
            yield break;
        }

        // 🔹 Eğer path yoksa, sahnede otomatik bulmayı dene
        if (enemyPathPoints == null || enemyPathPoints.Length == 0)
        {
            Transform pathRoot = transform.parent != null ?
                transform.parent.Find("Path") :
                GameObject.Find("Path")?.transform;

            if (pathRoot != null)
            {
                List<Transform> temp = new List<Transform>();
                foreach (Transform t in pathRoot)
                    temp.Add(t);
                enemyPathPoints = temp.ToArray();
                Debug.Log($"🔍 Path otomatik bulundu: {enemyPathPoints.Length} nokta.");
            }
            else
            {
                Debug.LogError("❌ Enemy path bulunamadı! Lütfen SetPath() çağırın veya prefab içinde EnemyPath objesi oluşturun.");
                yield break;
            }
        }

        totalWaves = levelData.waves.Length;
        currentWaveIndex = 1;

        foreach (var wave in levelData.waves)
        {
            for (int i = 0; i < wave.dusmanSayisi; i++)
            {
                EnemyData randomEnemy = wave.enemyTypes[Random.Range(0, wave.enemyTypes.Length)];

                // 🔹 Düşmanı spawnla
                Vector3 spawnPos = (spawnPoint != Vector3.zero) ? spawnPoint : enemyPathPoints[0].position;
                spawnPos += new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));

                GameObject enemyObj = Instantiate(randomEnemy.prefab, spawnPos, Quaternion.identity);
                Enemy enemy = enemyObj.GetComponent<Enemy>();

                enemy.data = randomEnemy;
                enemy.SetPath(enemyPathPoints);
                enemy.OnEnemyDied += HandleEnemyDeath;

                activeEnemies.Add(enemyObj);

                yield return new WaitForSeconds(wave.dusmanSpawnSuresi);
            }

            

            yield return new WaitForSeconds(wave.dalgaArasiBeklemeSuresi);
            if (currentWaveIndex < totalWaves)
            {
                currentWaveIndex++;
                GameManager.Instance.UpdateUI();
            }
        }

        spawningFinished = true;
        Debug.Log("🎯 Tüm düşmanlar spawnlandı, bekleniyor...");
    }
    
    public void ClearEnemies()
    {
        foreach (var enemyObj in activeEnemies)
        {
            if (enemyObj != null)
                Destroy(enemyObj);
        }
        activeEnemies.Clear();
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy.gameObject))
            activeEnemies.Remove(enemy.gameObject);

        if (spawningFinished && activeEnemies.Count == 0)
        {
            Debug.Log("🏁 Level tamamlandı!");
            GameManager.Instance.LevelFinished();
        }
    }
}
