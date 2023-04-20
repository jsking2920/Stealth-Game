using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityMovementAI;
using Random = UnityEngine.Random;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager S;

    [Header("Game Mode Params")]
    public int playersPerTeam = 1; // 1 means free for all; teams will fill in the order joined
    public int numberOfNpcs = 140;
    public bool doNpcsRespawn = false; // unimplemented currently
    public List<string> startGameMessages; // one selected at random

    public float playerRespawnTime = 3.0f;
    
    // Must be present in scene, set at runtime
    [HideInInspector] public UIManager uiManager;
    [HideInInspector] public PlayerInputManager inputManager;
    [HideInInspector] public NPCManager npcManager;
    [HideInInspector] public SceneManager sceneManager;
    [HideInInspector] public ColorManager colorManager;
    // TODO: Implement different arenas
    // [HideInInspector] public GameObject arenaPrefab;

    [HideInInspector] public List<Team> teams = new List<Team>();
    [HideInInspector] public bool playerInteractionEnabled = false;

    [HideInInspector] public enum GameState { joining, playing, paused, ended }
    [HideInInspector] public GameState gameState;

    private void Awake()
    {
        if (S != null)
        {
            Debug.LogError("Two Game Managers!!!");
            Destroy(gameObject);
        }
        S = this;
    }

    // GameModeManager gets instantiated by SceneManager on scene load, then this gets called
    protected virtual void Start()
    {
        // Having more than one of any of these will cause problems
        uiManager = FindObjectOfType<UIManager>();
        inputManager = FindObjectOfType<PlayerInputManager>();
        npcManager = FindObjectOfType<NPCManager>();
        sceneManager = FindObjectOfType<SceneManager>();
        colorManager = FindObjectOfType<ColorManager>();

        if (!uiManager || !inputManager || !npcManager || !sceneManager || !colorManager)
        {
            Debug.LogError("Missing a manager in the scene");
        }
        
        gameState = GameState.joining;

        inputManager.onPlayerJoined += OnPlayerJoin;
        inputManager.EnableJoining();
    }
    
    protected virtual void Update()
    {
        if (gameState == GameState.playing && CheckEndCondition())
        {
            EndGame();
        }

        // Pause on esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        // Start game and stop allowing joining on Space
        else if (gameState == GameState.joining && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }
    
    private System.Random _random = new System.Random();
    
    void Shuffle<T>(List<T> list) {
        int p = list.Count;
        for (int n = p-1; n > 0 ; n--)
        {
            int r = _random.Next(1, n);
            (list[r], list[n]) = (list[n], list[r]);
        }
    }

    public virtual void StartGame()
    {
        inputManager.DisableJoining();
        npcManager.SpawnNPCs(numberOfNpcs);

        uiManager.OnGameStart(startGameMessages[Random.Range(0, startGameMessages.Count)]);
        gameState = GameState.playing;

        Shuffle(colorManager.currentColorProfile.teamAppearances);
        foreach (Team team in teams)
        {
            foreach (Player player in team.players)
            {
                colorManager.SetPlayerAppearance(player);
            }
        }
    }

    protected virtual void EndGame()
    {
        gameState = GameState.ended;
        npcManager.DestroyNPCs();

        Team winningTeam = GetWinningTeam();
        foreach (Team team in teams)
        {
            if (winningTeam != null && team.index != winningTeam.index)
            {
                team.DestroyPlayers();
            }
        }

        uiManager.OnGameEnd(GetWinMessage()); 
    }

    protected virtual string GetWinMessage()
    {
        return "You Win!";
    }

    public void TogglePause()
    {
        if (gameState == GameState.paused)
        {
            gameState = GameState.playing;
            uiManager.OnResume();
            Time.timeScale = 1.0f;
        }
        else if (gameState == GameState.playing)
        {
            gameState = GameState.paused;
            uiManager.OnPause();
            Time.timeScale = 0.0f; // will mess with animations and other things, watch out
        }
    }

    public void RestartGame()
    {
        sceneManager.btn_RestartGame();
    }

    public void QuitToMenu()
    {
        sceneManager.ToMenu();
    }

    public void QuitGame()
    {
        sceneManager.btn_ExitGame();
    }

    protected virtual bool CheckEndCondition()
    {
        return false;
    }

    protected virtual Team GetWinningTeam()
    {
        return null;
    }

    protected virtual void OnPlayerJoin(PlayerInput playerInput)
    {
        Player newPlayer = playerInput.gameObject.GetComponent<Player>();
        newPlayer.Setup();
        SetSpawnPosition(newPlayer.transform);
        Debug.Log("player join");

        // if theres at least 1 team and the most recently created team has less players than required
        if (teams.Count > 0 && teams[teams.Count - 1].players.Count < playersPerTeam)
        {
            Team t = teams[teams.Count - 1];
            t.AddPlayer(playerInput, newPlayer);
        }
        else
        {
            // Create a new team
            Team newTeam = new Team(playerInput, newPlayer, teams.Count);
            teams.Add(newTeam);
            uiManager.AddTeamScoreText(newTeam);
        }
    }

    // TODO: Allow different arenas to have predefined spawn positions
    public virtual void SetSpawnPosition(Transform playerTransform)
    {
        npcManager.RandomizePosition(playerTransform);
    }

    public virtual void OnPlayerKilledPlayer(Player killer, Player victim)
    {

    }

    public virtual void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        ParticleSystem particles = npc.gameObject.GetComponentInChildren<ParticleSystem>();
        var main = particles.main;
        main.startColor = npc.GetComponent<SpriteRenderer>().color;
        particles.Play();
        npcManager.RemoveNPC(npc);
        Destroy(npc.gameObject);

        if (doNpcsRespawn)
        {
            // TODO: implement
            //npcSpawner.SpawnNPC();
        }
    }
    
    public bool CheckIfThereArePlayersLeft(Team team)
    {
        List<Player> teamPlayers = team.players;
        bool playersLeft = false;
        foreach (Player player in teamPlayers)
        {
            if (player.lives > 0)
                playersLeft = true;
        }
        return playersLeft;
    }
}