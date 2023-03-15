using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class KingOfTheHillManager : GameModeManager
{
    public float duration = 60.0f;
    [HideInInspector] public float timeRemaining = 60.0f;

    protected override void Start()
    {
        base.Start();
    }

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
        base.StartGame();
    }

    protected override bool CheckEndCondition()
    {
        return timeRemaining <= 0.0f;
    }

    protected override void OnPlayerJoin(PlayerInput playerInput)
    {
        base.OnPlayerJoin(playerInput);
    }
}