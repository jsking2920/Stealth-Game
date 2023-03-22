using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public ColorData currentColorProfile;
    [SerializeField] private ColorData[] allColorProfiles;

    void SwitchColorProfile(int profileIndex)
    {
        currentColorProfile = allColorProfiles[profileIndex];
    }
    
    void LoadGame()
    {
        SetNPCColors();
        SetTeamColors();
    }

    void SetEnvironmentColors()
    {
        
    }

    void SetUIColors()
    {
        
    }

    // needs to be called before SpawnNPCs() on NPCManager is called
    void SetNPCColors()
    {
        //GameModeManager.S.npcManager.colorList = currentColorProfile.npcColors;
    }

    // sets all player colors to the colors of their team
    void SetTeamColors()
    {
        List<Team> teams = GameModeManager.S.teams;

        for (int i = 0; i < currentColorProfile.teamColors.Count; i++)
        {
            foreach (Player player in teams[i].players)
            {
                player.SetColor(currentColorProfile.teamColors[i]);
            }
        }
    }
}
