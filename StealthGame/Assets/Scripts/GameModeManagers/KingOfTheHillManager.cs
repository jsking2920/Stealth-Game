using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class KingOfTheHillManager : GameModeManager
{
    public float duration = 60.0f;
    [HideInInspector] public float timeRemaining = 60.0f;

    public float npcKillPenalty = 3.0f;

    protected override void Start()
    {
        base.Start();
        uiManager.useFloatScore = true;
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
        uiManager.OnGameEnd("You Win\n" + GetWinningTeam().floatScore); // messy
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
            if (team.floatScore > winningTeam.floatScore)
            {
                winningTeam = team;
            }
        }

        return winningTeam;
    }

    public override void OnPlayerKilledNPC(Player killer)
    {
        base.OnPlayerKilledNPC(killer);

        teams[killer.teamIndex].floatScore -= npcKillPenalty;
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].floatScore.ToString());
    }

    public override void OnPlayerKilledPlayer(Player killer, Player victim)
    {
        base.OnPlayerKilledPlayer(killer, victim);
    }
}