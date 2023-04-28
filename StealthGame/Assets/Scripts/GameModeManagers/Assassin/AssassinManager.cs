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

    protected override bool CheckReadyToStart()
    {
        int playersJoined = 0;
        foreach (Team t in teams)
        {
            playersJoined += t.playerCount;
        }

        if (playersJoined < 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    protected override void NotReadyWarning()
    {
        uiManager.NotReadyWarning("2 Players Minimum Required!");
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
        if (npc.gameObject == curTarget.gameObject)
        {
            teams[killer.teamIndex].intScore += assassinationValue;
            killer.PlayScoreFeedback(npc.gameObject.transform.position, assassinationValue, true);
        }   
        else
        {
            if (curScore == 0)
            {
                killer.PlayScoreFeedback(npc.gameObject.transform.position, 0, false);
            }
            else
            {
                teams[killer.teamIndex].intScore = teams[killer.teamIndex].intScore - npcKillPenalty;
                killer.PlayScoreFeedback(npc.gameObject.transform.position, npcKillPenalty, false);
            }
            
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

        int curScore = teams[killer.teamIndex].intScore;

        if (killerTeam.index == victimTeam.index)
        {
            // team kill
            if (curScore == 0)
            {
                killer.PlayScoreFeedback(victim.gameObject.transform.position, 0, false);
            }
            else
            {
                killerTeam.intScore = Mathf.Clamp(killerTeam.intScore - teamKillPenalty, 0, 10000);
                killer.PlayScoreFeedback(victim.gameObject.transform.position, teamKillPenalty, false);
            }
        }
        else
        {
            killerTeam.intScore += playerKillValue;
            killer.PlayScoreFeedback(victim.gameObject.transform.position, playerKillValue, true);
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
