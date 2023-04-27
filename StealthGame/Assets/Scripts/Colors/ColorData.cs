using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/Color", order = 1)]
public class ColorData : ScriptableObject
{
    [System.Serializable]
    public class EnvironmentColors
    {
        public Color background;
        public Color obstacles;
        public Color interactables;
    }
    
    [System.Serializable]
    public class UIColors
    {
        public Color overlay;
        public Color button;
        public Color nonClickable;
        public Color text;
    }

    [System.Serializable]
    public class PlayerAppearance
    {
        public Color color;
        public string colorName;
        public Sprite sprite;
    }
    
    public List<PlayerAppearance> teamAppearances;
    public List<PlayerAppearance> playerAppearances;
    public List<Color> npcColors;
    public List<Sprite> npcSprites;
    public EnvironmentColors environmentColors;
    public UIColors uiColors;
}
