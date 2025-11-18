using UnityEngine;

[CreateAssetMenu(fileName = "TileMapData", menuName = "Tower Defence/TileMap Data")]
public class TileMapData : ScriptableObject
{
    public GameObject mapPrefab;
    public Vector3 spawnPoint;
    public Transform[] enemyPathPoints;
}

