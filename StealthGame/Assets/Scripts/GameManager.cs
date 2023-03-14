using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public PlayerInputManager inputManager;
    public NPCSpawner npcSpawner;
    public GameObject arenaPrefab; // TODO: implement ability to choose between multiple arenas
}
