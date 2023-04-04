using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;
using Random = UnityEngine.Random;

public class DeathmatchManager : TimedGameMode
{
    [Header("Deathmatch Settings")]
    public int killValue = 3;
    public int npcKillPenalty = 1;
    public int teamKillPenalty = 2;

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
            killerTeam.intScore += killValue;
        }
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
        
        RandomizeColorForFFA(victim);
    }

    public void RandomizeColorForFFA(Player player)
    {
        if (playersPerTeam == 1)
        {
            List <Color> colors = colorManager.currentColorProfile.npcColors;
            int rand = Random.Range(0, colors.Count);
            player.SetColor(colors[rand]);
        }
    }
}
