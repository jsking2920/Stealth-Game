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

    void SetEnvironmentColors()
    {
        
    }

    void SetUIColors()
    {
        
    }

    // sets all player appearance to the colors and sprites of their team
    public void SetPlayerAppearance(Player player)
    {
        Debug.Log("Set Appearance");
        ColorData.PlayerAppearance reference = currentColorProfile.teamAppearances[player.teamIndex];
        
        if (teamMatch == TeamMatchToAppearance.Color || teamMatch == TeamMatchToAppearance.SpriteAndColor)
            player.SetColor(reference.color);
        if (teamMatch == TeamMatchToAppearance.Sprite || teamMatch == TeamMatchToAppearance.SpriteAndColor)
            player.gameObject.GetComponent<SpriteRenderer>().sprite = reference.sprite;
    }

    public void SetLobbyAppearance(Player player)
    {
        ColorData.PlayerAppearance reference = currentColorProfile.teamAppearances[0];
        player.SetColor(Color.gray);
        player.gameObject.GetComponent<SpriteRenderer>().sprite = reference.sprite;
    }

    public Color SetTeamColor(Player player)
    {
        ColorData.PlayerAppearance reference = currentColorProfile.teamAppearances[player.teamIndex];
        return reference.color;
    }
}
