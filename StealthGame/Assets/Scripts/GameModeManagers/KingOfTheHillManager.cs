using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheHillManager : GameModeManager
{
    public float duration = 60.0f;
    [HideInInspector] public float timeRemaining;
    
    protected override void Start()
    {
        base.Start();

        timeRemaining = duration + uiManager.cutToBlackTime;
    }

    protected override void StartGame()
    {
        base.StartGame();

        uiManager.ShowTimerText();
    }

    protected override void Update()
    {
        base.Update();

        if (playerInteractionEnabled) timeRemaining -= Time.deltaTime;
    }

    protected override bool CheckEndCondition()
    {
        return timeRemaining <= 0.0f;
    }
}
