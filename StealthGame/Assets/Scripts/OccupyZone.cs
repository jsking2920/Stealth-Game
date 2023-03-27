using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupyZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player occupant = other.GetComponent<Player>();
            if (!occupant._objectiveCompleted && occupant._objective == Player.ObjectiveType.OccupyZone)
            {
                occupant._occupyZoneCurTime += Time.deltaTime;
            }
        }
    }
}
