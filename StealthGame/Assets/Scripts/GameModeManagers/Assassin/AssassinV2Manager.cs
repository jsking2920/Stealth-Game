using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;
using UnityEngine.InputSystem;

public class AssassinV2Manager : TimedGameMode
{
    [Header("AssassinV2 Settings")]
    [HideInInspector] public MovementAIRigidbody curTarget = null; // randomly selected npc

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
        return "You Win\n" + GetWinningTeam().index;
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
        if (teams.Count > 0 && (timeRemaining <= 0.0f || CheckIfThereArePlayersLeft(teams[0])))
            return true;
        return false;
    }

    public override void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        // experimental: don't decrement score for player with the kill NPC objective
        if (killer._objective != Player.ObjectiveType.KillNpcs) 
            teams[killer.teamIndex].intScore -= npcKillPenalty;

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
            victim.eliminated = true;
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
            
            // set victims colors
            if (teams.Count - 1 == 0)
            {
                Instantiate(assassinMarkPrefab, newPlayer.transform);
                npcManager.RandomizeColor(newPlayer.gameObject.GetComponent<SpriteRenderer>());
            }

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
}
