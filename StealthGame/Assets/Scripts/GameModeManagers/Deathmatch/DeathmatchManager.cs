using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class DeathmatchManager : GameModeManager
{
    [Header("Deathmatch Settings")]
    public float duration = 60.0f;
    [HideInInspector] public float timeRemaining = 60.0f;

    public int npcKillPenalty = 1;
    public int teamKillPenealty = 2;

    protected override void Start()
    {
        base.Start();
        uiManager.useFloatScore = false;
    }

    protected override void Update()
    {
        base.Update();

        if (gameState == GameState.playing)
        {
            timeRemaining -= Time.deltaTime;
            uiManager.SetTimerText(timeRemaining);
        }
    }

    protected override void StartGame()
    {
        timeRemaining = duration + uiManager.cutToBlackTime;
        base.StartGame();
    }

    protected override void EndGame()
    {
        base.EndGame();
        uiManager.OnGameEnd("You Win\n" + GetWinningTeam().intScore + " Kills"); // messy
    }

    protected override bool CheckEndCondition()
    {
        return timeRemaining <= 0.0f;
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
            killerTeam.intScore -= teamKillPenealty;
        }
        else
        {
            killerTeam.intScore++;
        }
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
    }
}
