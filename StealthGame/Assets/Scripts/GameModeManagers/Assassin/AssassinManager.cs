using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;
using Vector3 = System.Numerics.Vector3;

public class AssassinManager : TimedGameMode
{
    [Header("Assassin Settings")]
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

    protected override string GetWinMessage()
    {
        return base.GetWinMessage() + "\n" + GetWinningTeam().intScore + " Kills";
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
        if (npc.gameObject == curTarget.gameObject)
        {
            teams[killer.teamIndex].intScore += assassinationValue;
        }   
        else
        {
            teams[killer.teamIndex].intScore = Mathf.Clamp(teams[killer.teamIndex].intScore - npcKillPenalty, 0, 10000);
            killer.OnStabbed(null); // killing wrong target forces you to respawn
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
            // team kill
            killerTeam.intScore = Mathf.Clamp(killerTeam.intScore - teamKillPenalty, 0, 10000);
        }
        else
        {
            killerTeam.intScore += playerKillValue;
        }
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
    }

    public void SetRandomTarget()
    {
        curTarget = npcManager.npcs[Random.Range(0, npcManager.npcs.Count)];
        Instantiate(assassinMarkPrefab, curTarget.transform);

        curTarget.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
