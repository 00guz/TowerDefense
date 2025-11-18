using UnityEngine;

public class LevelResetter : MonoBehaviour
{
    [Tooltip("Sahnede silinecek kulelerin tag'ı")]
    public string towerTag = "Tower";
    public int startingHealth = 10;
    public static LevelResetter Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetLevel()
    {
        // 🔹 SADECE Kuleleri sil. Düşmanları ve haritayı GameManager halledecek.
        GameObject[] towers = GameObject.FindGameObjectsWithTag(towerTag);
        foreach (GameObject tower in towers)
            Destroy(tower);

        // 🔹 Oyuncunun durumunu sıfırla
        if (GameManager.Instance != null)
        {
            GameManager.Instance.can = startingHealth;
            // Not: UpdateUI() veya StartLevel() çağırmaya gerek yok,
            // GameManager bunu zaten yapacak.
        }

        Debug.Log("🔄 Kuleler ve oyuncu canı sıfırlandı!");
    }
}