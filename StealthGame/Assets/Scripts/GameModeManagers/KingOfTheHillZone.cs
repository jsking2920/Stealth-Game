using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheHillZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameModeManager.S.gameState == GameModeManager.GameState.playing)
        {
            // disgusting; doing this every frame is awful
            int index = collision.gameObject.GetComponent<Player>().teamIndex;
            GameModeManager.S.teams[index].floatScore += Time.deltaTime;
            GameModeManager.S.uiManager.UpdateTeamScore(index, GameModeManager.S.teams[index].floatScore.ToString("F2")); // this shouldn't be here
        }
    }
}
