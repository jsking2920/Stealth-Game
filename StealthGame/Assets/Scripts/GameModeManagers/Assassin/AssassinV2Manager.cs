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

    // teamIndex 0 are victims, teamIndex 1 are the assassins
    protected override void Start()
    {
        base.Start();
        uiManager.useFloatScore = false;
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
        else 
            return teams[1];
    }

    protected override bool CheckEndCondition()
    {
        return base.CheckEndCondition() || (teams.Count > 0 && !CheckIfThereArePlayersLeft(teams[0]));
    }

    public override void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        teams[killer.teamIndex].intScore -= npcKillPenalty;
        killer.OnStabbed(null); // killing wrong target forces you to respawn

        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());

        base.OnPlayerKilledNPC(killer, npc);
    }

    public override void OnPlayerKilledPlayer(Player killer, Player victim)
    {
        base.OnPlayerKilledPlayer(killer, victim);

        Team killerTeam = teams[killer.teamIndex];
        Team victimTeam = teams[victim.teamIndex];

        if (killerTeam.index == victimTeam.index)
        {
            // team kill
            killerTeam.intScore -= teamKillPenalty;
        }
        else
        {
            victim.lives -= 1;
        }
 
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
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
            if (newPlayer.teamIndex == 0)
                newPlayer.canStab = false;
        }
        else
        {
            // Create a new team
            Team newTeam = new Team(playerInput, newPlayer, teams.Count);

            // set killer color
            if (newPlayer.teamIndex == 0)
            {
                newPlayer.canStab = false;
            }
            else
            {
                newPlayer.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                newTeam.teamColor = Color.white;
                Instantiate(assassinMarkPrefab, newPlayer.transform);
            }

            teams.Add(newTeam);
            uiManager.AddTeamScoreText(newTeam);
        }
    }
}
