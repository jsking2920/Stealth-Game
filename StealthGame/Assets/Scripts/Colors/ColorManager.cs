using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public List<ColorData.PlayerAppearance> teamAppearances; 
    [SerializeField] private ColorData[] allColorProfiles;

    private void Start()
    {
        teamAppearances = currentColorProfile.teamAppearances;
        Shuffle();
    }

    void Shuffle()
    {
        List<ColorData.PlayerAppearance> skins = teamAppearances;
        for (int i = 0; i < skins.Count; i++) {
            ColorData.PlayerAppearance temp = skins[i];
            int randomIndex = Random.Range(i, skins.Count);
            skins[i] = skins[randomIndex];
            skins[randomIndex] = temp;
        }
    }
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
        ColorData.PlayerAppearance reference = teamAppearances[player.teamIndex];
        
        if (teamMatch == TeamMatchToAppearance.Color || teamMatch == TeamMatchToAppearance.SpriteAndColor)
            player.SetColor(reference.color);
        if (teamMatch == TeamMatchToAppearance.Sprite || teamMatch == TeamMatchToAppearance.SpriteAndColor)
            player.gameObject.GetComponent<SpriteRenderer>().sprite = reference.sprite;
    }

    public void SetLobbyAppearance(Player player)
    {
        int rand = Random.Range(0, teamAppearances.Count);
        ColorData.PlayerAppearance reference = teamAppearances[rand];
        player.SetColor(new Color(0.3f, 0.3f, 0.3f, 1.0f));
        player.gameObject.GetComponent<SpriteRenderer>().sprite = reference.sprite;
    }

    public Color SetTeamColor(Player player)
    {
        ColorData.PlayerAppearance reference = teamAppearances[player.teamIndex];
        return reference.color;
    }
}
