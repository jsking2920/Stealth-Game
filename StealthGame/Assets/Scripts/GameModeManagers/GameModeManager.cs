using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityMovementAI;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager S;

    [Header("Game Mode Params")]
    public int playersPerTeam = 1; // 1 means free for all; teams will fill in the order joined
    public int numberOfNPCs = 140;
    public List<string> startGameMessages;
    public bool showTimer = false;
    public float playerRespawnTime = 3.0f;

    [Header("Managers and Game Objects")]
    public UIManager uiManager;
    public PlayerInputManager inputManager;
    public NPCSpawner npcSpawner;
    // TODO: Implement different arenas
    // public GameObject arenaPrefab;

    [HideInInspector] public List<Team> teams = new List<Team>();
    [HideInInspector] public bool playerInteractionEnabled = false;

    [HideInInspector] public enum GameState { joining, playing, ended }
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

    protected virtual void Start()
    {
        gameState = GameState.joining;
        inputManager.onPlayerJoined += OnPlayerJoin;
        inputManager.EnableJoining();
        uiManager.SetPreGameUI();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.S.ToMenu();
        }

        if (gameState == GameState.joining && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
        else if (gameState == GameState.playing && CheckEndCondition())
        {
            EndGame();
        }
    }

    protected virtual void StartGame()
    {
        playerInteractionEnabled = true;
        inputManager.DisableJoining();
        npcSpawner.SpawnNPCs(numberOfNPCs);

        uiManager.OnGameStart(startGameMessages[Random.Range(0, startGameMessages.Count)], showTimer);
        gameState = GameState.playing;
    }

    protected virtual void EndGame()
    {
        gameState = GameState.ended;
        playerInteractionEnabled = false;
        npcSpawner.DestroyNPCs();
        Team winningTeam = GetWinningTeam();
        foreach (Team team in teams)
        {
            if (winningTeam != null && team.index != winningTeam.index)
            {
                team.DestroyPlayers();
            }
        }
        uiManager.OnGameEnd(); // Call this in override to set a custom message
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

    public virtual void OnPlayerKilledPlayer(Player killer, Player victim)
    {

    }

    public virtual void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        NPCSpawner.S.RemoveNPC(npc);
        Destroy(npc.gameObject);
    }
}

[System.Serializable]
public class Team
{
    public int index;
    
    public int playerCount;
    public float floatScore;
    public int intScore;

    public Color color;

    public List<PlayerInput> playerInputs;
    public List<Player> players;

    public Team(PlayerInput input, Player p, int i)
    {
        players = new List<Player>();
        players.Add(p);
        playerInputs = new List<PlayerInput>();
        playerInputs.Add(input);

        playerCount = 1;
        floatScore = 0.0f;
        intScore = 0;

        index = i;

        // Color of team defaults to first player added to that team
        p.RandomizeColor();
        color = p.color;

        p.teamIndex = i;
    }

    public void AddPlayer(PlayerInput input, Player p)
    {
        players.Add(p);
        playerInputs.Add(input);
        playerCount++;
        p.SetColor(color);
        p.teamIndex = index;
    }

    public void DestroyPlayers()
    {
        foreach (Player p in players)
        {
            UnityEngine.GameObject.Destroy(p.gameObject);
        }
        players.Clear();
    }
}