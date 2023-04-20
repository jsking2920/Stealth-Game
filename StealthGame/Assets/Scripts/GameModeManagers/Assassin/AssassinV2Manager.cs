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

    public override void StartGame()
    {
        base.StartGame();
        List<Player> assassins = teams[1].players;
        foreach (Player killer in assassins)
        {
            killer.SetColor(assassinColor);
        }
    }

    protected override string GetWinMessage()
    {
        return GetWinningTeam().index == 0 ? "Victims Win!" : "Assassins Win!";
    }

    protected override Team GetWinningTeam()
    {
        //if team 0 is all dead, team 1 wins 
        //if team 0 is not all dead, team 0 wins, 
        
        if (CheckIfThereArePlayersLeft(teams[0]))
            return teams[0]; 
        return teams[1];
    }

    protected override bool CheckEndCondition()
    {
        return base.CheckEndCondition() || (teams.Count > 0 && (!CheckIfThereArePlayersLeft(teams[0]) || !CheckIfThereArePlayersLeft(teams[1])));
    }

    public override void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        killer.lives--;
        killer.OnStabbed(null); // killing wrong target forces you to respawn

        uiManager.UpdateLivesText(killer.playerIndex, (killer.lives + 1).ToString());

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
 
        uiManager.UpdateLivesText(killer.playerIndex, (killer.lives + 1).ToString());
    }

    protected override void OnPlayerJoin(PlayerInput playerInput)
    {
        Player newPlayer = playerInput.gameObject.GetComponent<Player>();
        newPlayer.Setup();

        SetSpawnPosition(newPlayer.transform);

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
        }
        else
        {
            newPlayer._maxVelocity += assassinSpeedIncrease;
            uiManager.AddPlayerLivesText(newPlayer);
        }
    }
}
