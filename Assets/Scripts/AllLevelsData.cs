using UnityEngine;

[CreateAssetMenu(fileName = "AllLevelsDatabase", menuName = "Tower Defence/All Levels Database")]
public class AllLevelsData : ScriptableObject
{
    public LevelData[] allLevels;
}