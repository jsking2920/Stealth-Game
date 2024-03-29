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

    protected override bool CheckReadyToStart()
    {
        int playersJoined = 0;
        foreach (Team t in teams)
        {
            playersJoined += t.playerCount;
        }

        if (playersPerTeam == 1 && playersJoined > 1)
        {
            return true;
        }
        else if (playersPerTeam == 2 && playersJoined > 2)
        {
            return true;
        }
        else if (playersPerTeam == 3 && playersJoined > 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void NotReadyWarning()
    {
        int playersJoined = 0;
        foreach (Team t in teams)
        {
            playersJoined += t.playerCount;
        }

        if (playersPerTeam == 1 && playersJoined < 2)
        {
            uiManager.NotReadyWarning("2 Players Minimum Required!");
        }
        else if (playersPerTeam == 2 && playersJoined < 3)
        {
            uiManager.NotReadyWarning("3 Players Minimum Required!");
        }
        else if (playersPerTeam == 3 && playersJoined < 4)
        {
            uiManager.NotReadyWarning("4 Players Minimum Required!");
        }
        else
        {
            uiManager.NotReadyWarning("Not Ready");
        }
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
        int curScore = teams[killer.teamIndex].intScore;

        if (curScore == 0)
        {
            killer.PlayScoreFeedback(npc.gameObject.transform.position, 0, false);
        }
        else
        {
            teams[killer.teamIndex].intScore = Mathf.Clamp(teams[killer.teamIndex].intScore - npcKillPenalty, 0, 10000);
            killer.PlayScoreFeedback(npc.gameObject.transform.position, npcKillPenalty, false);
        }
        
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
            if (teams[killer.teamIndex].intScore == 0)
            {
                killer.PlayScoreFeedback(victim.gameObject.transform.position, 0, false);
            }
            else
            {
                killer.PlayScoreFeedback(victim.gameObject.transform.position, teamKillPenalty, false);
            }
            // team kill
            killerTeam.intScore = Mathf.Clamp(killerTeam.intScore - teamKillPenalty, 0, 10000);
        }
        else
        {
            killerTeam.intScore += killValue;
            killer.PlayScoreFeedback(victim.gameObject.transform.position, killValue, true);
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
            teams[player.teamIndex].teamColor = colors[rand];
            uiManager.teamTexts[player.teamIndex].color = colors[rand];
            uiManager.teamImages[player.teamIndex].color = colors[rand];
        }
    }
}
