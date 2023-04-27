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
        return base.GetWinMessage() + "\n" + "Score: " + GetWinningTeam()[0].intScore;
    }

    protected override List<Team> GetWinningTeam()
    {
        Team winningTeam = teams[0];
        List<Team> winningTeams = new List<Team>();

        foreach (Team team in teams)
        {
            if (team.intScore > winningTeam.intScore)
            {
                winningTeam = team;
            }
        }

        foreach (Team team in teams)
        {
            if (team.intScore == winningTeam.intScore)
                winningTeams.Add(team);
        }

        return winningTeams;
    }

    public override void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        teams[killer.teamIndex].intScore = Mathf.Clamp(teams[killer.teamIndex].intScore - npcKillPenalty, 0, 10000);
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
        
        killer.PlayScoreFeedback(npcKillPenalty, false);

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
            killerTeam.intScore = Mathf.Clamp(killerTeam.intScore - teamKillPenalty, 0, 10000);
            
            killer.PlayScoreFeedback(teamKillPenalty, false);
        }
        else
        {
            killerTeam.intScore += killValue;
            
            killer.PlayScoreFeedback(killValue, true);
        }
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
    }

    public override void OnRespawn(Player p)
    {
        base.OnRespawn(p);
        RandomizeColorForFFA(p);
    }

    public void RandomizeColorForFFA(Player player)
    {
        if (playersPerTeam == 1)
        {
            List <Color> colors = colorManager.currentColorProfile.npcColors;
            int rand = Random.Range(0, colors.Count);
            player.SetColor(colors[rand]);
            uiManager.teamTexts[player.teamIndex].color = colors[rand];
            uiManager.teamImages[player.teamIndex].color = colors[rand];
        }
    }
}
