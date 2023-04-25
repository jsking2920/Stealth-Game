using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Team
{
    public int index; // Needs to be unique, and increment up from 0, set on team creation
    public int playerIndex;

    public int playerCount;

    public float floatScore;
    public int intScore;

    public Color teamColor; // All players share a color

    public List<PlayerInput> playerInputs;
    public List<Player> players;

    void Start()
    {
        playerIndex = 0;
    }
    
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

        p.teamIndex = i;
        p.playerIndex = playerIndex;
        playerIndex++;

        teamColor = GameModeManager.S.colorManager.SetTeamColor(p);
        GameModeManager.S.colorManager.SetLobbyAppearance(p);
    }

    public void AddPlayer(PlayerInput input, Player p)
    {
        players.Add(p);
        playerInputs.Add(input);
        playerCount++;
        // p.SetColor(teamColor);
        p.teamIndex = index;
        p.playerIndex = playerIndex;
        playerIndex++;
        GameModeManager.S.colorManager.SetLobbyAppearance(p);
        // GameModeManager.S.colorManager.SetPlayerAppearance(p);
    }

    public void DestroyPlayers()
    {
        foreach(PlayerInput pi in playerInputs)
        {
            pi.DeactivateInput();
        }
        foreach (Player p in players)
        {
            GameObject.Destroy(p.gameObject);
        }
        players.Clear();
    }
}
