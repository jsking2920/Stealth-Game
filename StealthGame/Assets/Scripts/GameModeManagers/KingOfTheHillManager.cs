using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class KingOfTheHillManager : GameModeManager
{
    [Header("King of the Hill Settings")]
    public float duration = 60.0f;
    [HideInInspector] public float timeRemaining = 60.0f;

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
            timeRemaining -= Time.deltaTime;
            uiManager.SetTimerText(timeRemaining);

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
        timeRemaining = duration + uiManager.cutToBlackTime;
        _zoneRespawnTimer = zoneRespawnRate;
        
        base.StartGame();
        SpawnZone();
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
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].floatScore.ToString("F2"));
    }

    public override void OnPlayerKilledPlayer(Player killer, Player victim)
    {
        base.OnPlayerKilledPlayer(killer, victim);
    }

    private void SpawnZone()
    {
        Transform newZone = Instantiate(_zonePrefab, npcSpawner.GetRandomPos(1.0f, 2.0f), Quaternion.identity).transform;
        float s = Random.Range(zoneScaleMin, zoneScaleMax);
        newZone.localScale = new Vector3(s, s, 1.0f);
        _curZone = newZone.gameObject;
    }
}