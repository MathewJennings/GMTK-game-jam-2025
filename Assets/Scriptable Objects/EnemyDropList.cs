using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDropList", menuName = "Scriptable Objects/EnemyDropList", order = 1)]
public class EnemyDropList : ScriptableObject
{
    public List<GameObject> enemyDropPrefabs;
}
