using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    public Transform[] Waypoints => waypoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
