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
    public String _name;
    public List<String> scoringRules;
    public List<bool> ruleIsGood; // true = green, false = red

    public float playerRespawnTime = 3.0f;
    
    // Must be present in scene, set at runtime
    [HideInInspector] public UIManager uiManager;
    [HideInInspector] public PlayerInputManager inputManager;
    [HideInInspector] public NPCManager npcManager;
    [HideInInspector] public SceneManager sceneManager;
    [HideInInspector] public ColorManager colorManager;
    [HideInInspector] public ArenaManager arenaManager;
    [HideInInspector] public AudioManager audioManager;
    // TODO: Implement different arenas
    // [HideInInspector] public GameObject arenaPrefab;

    public List<Team> teams = new List<Team>();
    private List<Team> winningTeam = null;
    [HideInInspector] public bool playerInteractionEnabled = false;

    [HideInInspector] public enum GameState { joining, playing, paused, ended }
    [HideInInspector] public GameState gameState;

    private void Awake()
    {
        if (S != null)
        {
            Debug.Log("Game manager singleton being replaced");
            Destroy(S.gameObject);
        }
        S = this;

        // Having more than one of any of these will cause problems
        uiManager = FindObjectOfType<UIManager>();
        inputManager = FindObjectOfType<PlayerInputManager>();
        npcManager = FindObjectOfType<NPCManager>();
        sceneManager = FindObjectOfType<SceneManager>();
        colorManager = FindObjectOfType<ColorManager>();
        arenaManager = FindObjectOfType<ArenaManager>();
        audioManager = FindObjectOfType<AudioManager>();
        
        if (!uiManager || !inputManager || !npcManager || !sceneManager || !colorManager)
        {
            Debug.LogError("Missing a manager in the scene");
        }
    }

    // GameModeManager gets instantiated by SceneManager on scene load, then this gets called
    protected virtual void Start()
    {
        gameState = GameState.joining;
        uiManager.OnLobbyEnter(_name, scoringRules, ruleIsGood);

        inputManager.onPlayerJoined += OnPlayerJoin;
        inputManager.EnableJoining();

        arenaManager.InitializeArena();

        audioManager.PlayGameMusic();
        
        Shuffle(colorManager.currentColorProfile.teamAppearances);
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

    public virtual bool StartGame()
    {
        if (CheckReadyToStart())
        {
            inputManager.DisableJoining();
            npcManager.SpawnNPCs(numberOfNpcs);
            winningTeam = null;

            uiManager.OnGameStart(startGameMessages[Random.Range(0, startGameMessages.Count)]);
            gameState = GameState.playing;
            return true;
        }
        else
        {
            NotReadyWarning();
            return false;
        }
    }

    protected virtual bool CheckReadyToStart()
    {
        return true;
    }

    protected virtual void NotReadyWarning()
    {
        uiManager.NotReadyWarning("Not Ready!");
    }

    protected virtual void EndGame()
    {
        gameState = GameState.ended;
        winningTeam = GetWinningTeam();

        npcManager.DestroyNPCs(winningTeam[0].players[0]);

        foreach (Team team in teams)
        {
            bool won = false;
            foreach (Team winner in winningTeam)
            {
                if (team.index == winner.index)
                    won = true;
            }
            if (!won)
            {
                DestroyPlayersInTeam(team);
            }
        }

        uiManager.OnGameEnd(GetWinMessage()); 
    }

    protected virtual string GetWinMessage()
    {
        string winningColorName = "";
        foreach (ColorData.PlayerAppearance colorData in colorManager.currentColorProfile.teamAppearances)
        {
            Color profileColor = colorData.color;
            Color winningColor = winningTeam[0].teamColor;
            if (profileColor.Equals(winningColor))
            {
                winningColorName = colorData.colorName;
            }
        }

        if (winningTeam.Count > 1)
            return "Draw!";
        return winningColorName + " Wins!";
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
        if (winningTeam != null)
        {
            foreach (Team team in winningTeam)
                DestroyPlayersInTeam(team);
        }
        Time.timeScale = 1.0f;
        sceneManager.btn_RestartGame();
    }

    public void DestroyPlayersInTeam(Team t)
    {
        for (int i = 0; i < t.playerCount; i++)
        {
            t.players[i].TurnRumbleOff();
            t.playerInputs[i].DeactivateInput();
            t.players[i].PlayExplosionSelf();
        }
        foreach (Player p in t.players)
        {
            Destroy(p.gameObject);
        }
        t.players.Clear();
    }

    public void QuitToMenu()
    {
        if (winningTeam != null)
        {
            foreach (Team team in winningTeam)
            {
                DestroyPlayersInTeam(team);
            }   
        }
        Time.timeScale = 1.0f;
        audioManager.PlayMenuMusic();
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

    protected virtual List<Team> GetWinningTeam()
    {
        return null;
    }

    protected virtual void OnPlayerJoin(PlayerInput playerInput)
    {
        Player newPlayer = playerInput.gameObject.GetComponent<Player>();
        newPlayer.Setup();

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
        SetSpawnPosition(newPlayer);
    }

    // TODO: Allow different arenas to have predefined spawn positions
    public virtual void SetSpawnPosition(Player p)
    {
        Camera cam = Camera.main;
        float depth = p.transform.position.z - cam.transform.position.z;

        Vector3 lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, depth)) + new Vector3(3.0f, 3.0f, 0.0f);
        Vector3 lowerRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, depth)) + new Vector3(-3.0f, 3.0f, 0.0f);
        Vector3 upperLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, depth)) + new Vector3(3.0f, -3.0f, 0.0f);
        Vector3 upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth)) + new Vector3(-3.0f, -3.0f, 0.0f);

        switch (p.teamIndex)
        {
            case 0:
                p.transform.position = upperLeft;
                break;
            case 1:
                p.transform.position = upperRight;
                break;
            case 2:
                p.transform.position = lowerLeft;
                break;
            case 3:
                p.transform.position = lowerRight;
                break;
            case 4:
                npcManager.RandomizePosition(p.transform);
                break;
            case 5:
                npcManager.RandomizePosition(p.transform);
                break;
            default:
                npcManager.RandomizePosition(p.transform);
                break;
        }
    }

    public virtual void OnRespawn(Player p)
    {
        SetRepawnPosition(p);
    }

    public virtual void SetRepawnPosition(Player p)
    {
        npcManager.RandomizePosition(p.transform);
    }

    public virtual void OnPlayerKilledPlayer(Player killer, Player victim)
    {

    }

    public virtual void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
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