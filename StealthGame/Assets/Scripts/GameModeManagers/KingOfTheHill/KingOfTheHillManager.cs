using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class KingOfTheHillManager : TimedGameMode
{
    [Header("King of the Hill Settings")]
    public float npcKillPenalty = 3.0f;

    [SerializeField] private GameObject _zonePrefab;
    public float zoneScaleMin = 2.5f;
    public float zoneScaleMax = 5.0f;
    public float zoneRespawnRate = 10.0f;
    private float _zoneRespawnTimer = 0.0f;
    private GameObject _curZone;

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
            _zoneRespawnTimer -= Time.deltaTime;
            if (_zoneRespawnTimer <= 0.0f)
            {
                Destroy(_curZone);
                SpawnZone();
                _zoneRespawnTimer = zoneRespawnRate;
            }
        }
    }

    protected override void StartGame()
    {
        _zoneRespawnTimer = zoneRespawnRate;   
        base.StartGame();
        SpawnZone();
    }

    protected override string GetWinMessage()
    {
        return "You Win\n" + GetWinningTeam().floatScore;
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

    public override void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        teams[killer.teamIndex].floatScore -= npcKillPenalty;
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].floatScore.ToString("F2"));

        base.OnPlayerKilledNPC(killer, npc);
    }

    private void SpawnZone()
    {
        Transform newZone = Instantiate(_zonePrefab, npcManager.GetRandomPos(1.0f, 2.0f), Quaternion.identity).transform;
        float s = Random.Range(zoneScaleMin, zoneScaleMax);
        newZone.localScale = new Vector3(s, s, 1.0f);
        _curZone = newZone.gameObject;
    }
}