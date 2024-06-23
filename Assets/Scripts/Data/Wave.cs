using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave_", menuName = "Scriptable Objects/Gameplay/Wave")]
public class Wave : ScriptableObject
{
    public List<GameObject> Enemies = new List<GameObject>();
    public List<int> NumberEnemies = new List<int>();
    public int SpawnInterval = 0;

    public int TotalEnemies => NumberEnemies.Sum();
}
