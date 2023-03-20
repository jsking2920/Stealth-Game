using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMovementAI;

public class TimedGameMode : GameModeManager
{
    [Header("Timer Settings")]
    public float duration = 60.0f;
    [HideInInspector] public float timeRemaining = 60.0f;

    protected override void Update()
    {
        base.Update();

        if (gameState == GameState.playing)
        {
            timeRemaining -= Time.deltaTime;
            uiManager.SetTimerText(timeRemaining);
        }
    }

    protected override void StartGame()
    {
        timeRemaining = duration + uiManager.cutToBlackTime;
        uiManager.ShowTimerAfterCut();
        base.StartGame();
    }

    protected override bool CheckEndCondition()
    {
        return timeRemaining <= 0.0f;
    }
}
