using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class AssassinV2Manager : TimedGameMode
{
    [Header("AssassinV2 Settings")]
    // [HideInInspector] public MovementAIRigidbody curTarget = null; // randomly selected npc
    [SerializeField] private int numberOfFakeTargets;

    public GameObject assassinMarkPrefab;
    
    public int npcKillPenalty = 1;
    public int teamKillPenalty = 2;

    [SerializeField] private Color assassinColor = Color.white;
    [SerializeField] private float assassinSpeedIncrease = 0.30f;

    // teamIndex 0 are victims, teamIndex 1 are the assassins
    protected override void Start()
    {
        base.Start();
        uiManager.useFloatScore = false;
    }

    public override bool StartGame()
    {
        bool starting = base.StartGame();

        if (CheckReadyToStart())
        {
            List<Player> victims = teams[0].players;
            foreach (Player victim in victims)
            {
                GameModeManager.S.colorManager.SetPlayerAppearance(victim);
            }
        }
        return starting;
    }

    protected override bool CheckReadyToStart()
    {
        int playersJoined = 0;
        foreach (Team t in teams)
        {
            playersJoined += t.playerCount;
        }

        if (playersJoined < 3)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected override void NotReadyWarning()
    {
        uiManager.NotReadyWarning("3 Players Minimum Required!");
    }

    protected override string GetWinMessage()
    {
        return GetWinningTeam()[0].index == 0 ? "The Hunted Win!" : "The Hunters Win!";
    }

    protected override List<Team> GetWinningTeam()
    {
        List<Team> winners = new List<Team>();
        //if team 0 is all dead, team 1 wins 
        //if team 0 is not all dead, team 0 wins, 

        if (CheckIfThereArePlayersLeft(teams[0]))
        {
            winners.Add(teams[0]);
            return winners;
        }
        winners.Add(teams[1]);
        return winners;
    }

    protected override bool CheckEndCondition()
    {
        return base.CheckEndCondition() || (teams.Count >= 2 && (!CheckIfThereArePlayersLeft(teams[0]) || !CheckIfThereArePlayersLeft(teams[1])));
    }

    public override void SetSpawnPosition(Player p)
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
                npcManager.RandomizePosition(p.transform);
                break;
            case 1:
                p.transform.position = upperLeft;
                break;
            default:
                npcManager.RandomizePosition(p.transform);
                break;
        }
    }

    public override void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        killer.lives--;
        killer.OnStabbed(null); // killing wrong target forces you to respawn

        uiManager.UpdateLivesText(killer.playerIndex, (killer.lives).ToString());

        base.OnPlayerKilledNPC(killer, npc);
    }

    public override void OnPlayerKilledPlayer(Player killer, Player victim)
    {
        base.OnPlayerKilledPlayer(killer, victim);

        Team killerTeam = teams[killer.teamIndex];
        Team victimTeam = teams[victim.teamIndex];

        if (killerTeam.index != victimTeam.index)
        {
            victim.lives -= 1;
        }
    }

    protected override void OnPlayerJoin(PlayerInput playerInput)
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
        }
        
        if (newPlayer.teamIndex == 0) // victim setup
        {
            newPlayer.canStab = false;
            newPlayer.lives = 1;
            GameModeManager.S.colorManager.SetLobbyAppearance(newPlayer);
        }
        else
        {
            newPlayer._maxVelocity += assassinSpeedIncrease;
            uiManager.AddPlayerLivesText(newPlayer);
            newPlayer.SetColor(Color.white);
        }

        SetSpawnPosition(newPlayer);
    }
}
