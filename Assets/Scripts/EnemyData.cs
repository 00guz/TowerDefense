using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Tower Defence/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public float speed = 1f;
    public int maxHealth = 5;
    public int reward = 10;
    public int damageToBase = 1;

}
