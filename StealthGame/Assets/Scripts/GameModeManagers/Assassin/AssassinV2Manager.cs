using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class AssassinV2Manager : TimedGameMode
{
    [Header("AssassinV2 Settings")]
    // [HideInInspector] public MovementAIRigidbody curTarget = null; // randomly selected npc
    [SerializeField] private int numberOfFakeTargets;

    public GameObject assassinMarkPrefab;
    
    public int npcKillPenalty = 1;
    public int teamKillPenalty = 2;

    // teamIndex 0 are victims, teamIndex 1 are the assassins
    protected override void Start()
    {
        base.Start();
        uiManager.useFloatScore = false;
    }

    protected override void StartGame()
    {
        base.StartGame();
    }
    
    protected override string GetWinMessage()
    {
        return "You Win\n" + GetWinningTeam().index;
    }

    protected override Team GetWinningTeam()
    {
        //if team 0 is all dead, team 1 wins 
        //if team 0 is not all dead, team 0 wins, 
        
        if (CheckIfThereArePlayersLeft(teams[0]))
            return teams[0];
        else 
            return teams[1];
    }

    protected override bool CheckEndCondition()
    {
        return base.CheckEndCondition() || (teams.Count > 0 && !CheckIfThereArePlayersLeft(teams[0]));
    }

    public override void OnPlayerKilledNPC(Player killer, MovementAIRigidbody npc)
    {
        // experimental: don't decrement score for player with the kill NPC objective
        if (killer._objective != Player.ObjectiveType.KillNpcs) 
            teams[killer.teamIndex].intScore -= npcKillPenalty;
        killer.OnStabbed(null); // killing wrong target forces you to respawn

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
            victim.eliminated = true;
        }
 
        uiManager.UpdateTeamScore(killer.teamIndex, teams[killer.teamIndex].intScore.ToString());
    }

    protected override void OnPlayerJoin(PlayerInput playerInput)
    {
        Player newPlayer = playerInput.gameObject.GetComponent<Player>();
        newPlayer.Setup();

        SetSpawnPosition(newPlayer.transform);
        
        int rand = Random.Range(0, Enum.GetNames(typeof(Player.ObjectiveType)).Length);
        switch (rand)
        {
            case 0:
                newPlayer._objective = Player.ObjectiveType.DieToTeam;
                break;
            case 1:
                newPlayer._objective = Player.ObjectiveType.StandStill;
                break;
            case 2: 
                newPlayer._objective = Player.ObjectiveType.OccupyZone;
                break;
            case 3:
                newPlayer._objective = Player.ObjectiveType.KillNpcs;
                break;
            default:
                break;
        }

        // if theres at least 1 team and the most recently created team has less players than required
        if (teams.Count > 0 && teams[teams.Count - 1].players.Count < playersPerTeam)
        {
            Team t = teams[teams.Count - 1];
            
            

            t.AddPlayer(playerInput, newPlayer);
        }
        else
        {
            // Create a new team
            Team newTeam = new Team(playerInput, newPlayer, teams.Count);
            
            // set killer color
            if (teams.Count - 1 == 0)
            {
                newPlayer.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                //npcManager.RandomizeColor(newPlayer.gameObject.GetComponent<SpriteRenderer>());
            }
            
            teams.Add(newTeam);
            uiManager.AddTeamScoreText(newTeam);
        }
    }
    
    public void SetFakeTargets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            MovementAIRigidbody fake = npcManager.npcs[Random.Range(0, npcManager.npcs.Count)];
            Instantiate(assassinMarkPrefab, fake.transform);

            fake.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
