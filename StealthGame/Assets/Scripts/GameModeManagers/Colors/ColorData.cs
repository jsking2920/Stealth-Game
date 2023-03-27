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
    
    public List<Color> teamColors;
    public List<Color> playerColors;
    public List<Color> npcColors;
    public EnvironmentColors environmentColors;
    public UIColors uiColors;
}