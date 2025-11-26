using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Tower Defence/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public GameObject prefab;
    public Sprite icon;
    public int cost = 50;
    public float range = 3f;
    public float fireRate = 1f;
    public int damage = 2;
    public int soruSayisi = 1;

    [Header("Ses Ayarları")]
    [Tooltip("AudioManager'daki sesin sırası")]
    public int shootSoundIndex;
}
