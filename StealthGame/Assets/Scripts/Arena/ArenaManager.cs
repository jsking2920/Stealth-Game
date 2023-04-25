using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityMovementAI;

public class ArenaManager : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;

    public void InitializeArena()
    {
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Arena a = Instantiate(prefab).GetComponent<Arena>();
        if (a.obstacles.Length != 0)
        {
            GameModeManager.S.npcManager.obstacles.Concat<MovementAIRigidbody>(a.obstacles);
        }
    }
}
