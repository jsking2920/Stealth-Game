using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class AssassinManager : GameModeManager
{
    [Header("Assassin Settings")]
    public float duration = 60.0f;
    [HideInInspector] public float timeRemaining = 60.0f;

    public float timeToRespawnTarget = 3.0f;
    private float targetRespawnTimer = 3.0f;

    public int assassinationValue = 3;
    public int playerKillValue = 1;
    public int npcKillPenalty = 1;
    public int teamKillPenalty = 2;

    [HideInInspector] public MovementAIRigidbody curTarget = null; // randomly selected npc

    public GameObject assassinMarkPrefab;

    protected override void Start()
    {
        base.Start();
        uiManager.useFloatScore = false;
        targetRespawnTimer = timeToRespawnTarget;
    }

    protected override void Update()
    {
        base.Update();

        if (gameState == GameState.playing)
        {
            timeRemaining -= Time.deltaTime;
            uiManager.SetTimerText(timeRemaining);

            if (curTarget == null)
            {
                if (targetRespawnTimer <= 0.0f)
                {
                    SetRandomTarget();
                    targetRespawnTimer = timeToRespawnTarget;
                }
                else
                {
                    targetRespawnTimer -= Time.deltaTime;
                }
            }
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
        uiManager.OnGameEnd("You Win\n" + GetWinningTeam().intScore); // messy
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
        base.OnPlayerKilledNPC(killer, npc);

        if (npc.gameObject == curTarget.gameObject)
        {
            teams[killer.teamIndex].intScore += assassinationValue;
        }
        else
        {
            teams[killer.teamIndex].intScore -= npcKillPenalty;
        }

        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
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
            killerTeam.intScore += playerKillValue;
        }
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
    }

    public void SetRandomTarget()
    {
        curTarget = npcSpawner.npcs[Random.Range(0, npcSpawner.npcs.Count)];
        Instantiate(assassinMarkPrefab, curTarget.transform);

        curTarget.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
