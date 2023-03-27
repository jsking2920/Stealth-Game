using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class DeathmatchManager : TimedGameMode
{
    [Header("Deathmatch Settings")]
    public int npcKillPenalty = 1;
    public int teamKillPenealty = 2;

    protected override void Start()
    {
        base.Start();
        uiManager.useFloatScore = false;
    }

    protected override string GetWinMessage()
    {
        return "You Win\n" + GetWinningTeam().intScore + " Kills";
    }

    protected override Team GetWinningTeam()
    {
        Team winningTeam = teams[0];

        foreach (Team team in teams)
        {
            if (team.intScore > winningTeam.intScore)
            {
                winningTeam = team;
            }
        }

        return winningTeam;
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
            killerTeam.intScore -= teamKillPenealty;
        }
        else
        {
            killerTeam.intScore++;
        }
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
    }
}
