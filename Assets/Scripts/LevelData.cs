using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Tower Defence/Level Data")]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class Wave
    {
        public EnemyData[] enemyTypes;   // 🔹 Bu dalgadaki düşman türleri
        public int dusmanSayisi = 5;     // 🔹 Düşman sayısı
        public float dusmanSpawnSuresi = 1f;    // 🔹 Spawn arası süre
        public float dalgaArasiBeklemeSuresi = 5f;  // 🔹 Dalga arası bekleme süresi
    }

    [Header("Level Bilgisi")]
    public string levelName = "Level 1";
    public int startingGold = 100;   // 🔹 Başlangıç altını
    public Wave[] waves;

    [Header("Bu levelde kullanılabilir kuleler")]
    public TowerData[] allowedTowers;  // 🔹 Bu levelde kullanılabilecek kuleler
    public TileMapData tileMapData;   // 🔹 Bu levelin harita verisi
}
