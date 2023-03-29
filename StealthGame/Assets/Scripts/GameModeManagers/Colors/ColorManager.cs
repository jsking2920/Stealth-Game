using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    private enum TeamMatchToAppearance
    {
        Sprite,
        Color,
        SpriteAndColor,
    };

    [SerializeField] private TeamMatchToAppearance teamMatch;
    
    public ColorData currentColorProfile;
    [SerializeField] private ColorData[] allColorProfiles;

    void SwitchColorProfile(int profileIndex)
    {
        currentColorProfile = allColorProfiles[profileIndex];
    }
    
    public void LoadGame()
    {
        if (GameModeManager.S.teams.Count > 0)
            SetTeamAppearances();
    }

    void SetEnvironmentColors()
    {
        
    }

    void SetUIColors()
    {
        
    }

    // sets all player appearance to the colors and sprites of their team
    void SetTeamAppearances()
    {
        List<Team> teams = GameModeManager.S.teams;

        for (int i = 0; i < currentColorProfile.teamAppearances.Count; i++)
        {
            foreach (Player player in teams[i].players)
            {
                ColorData.PlayerAppearance reference = currentColorProfile.teamAppearances[i];
                if (teamMatch == TeamMatchToAppearance.Color || teamMatch == TeamMatchToAppearance.SpriteAndColor)
                    player.SetColor(reference.color);
                if (teamMatch == TeamMatchToAppearance.Sprite || teamMatch == TeamMatchToAppearance.SpriteAndColor)
                    player.gameObject.GetComponent<SpriteRenderer>().sprite = reference.sprite;
            }
        }
    }
}
