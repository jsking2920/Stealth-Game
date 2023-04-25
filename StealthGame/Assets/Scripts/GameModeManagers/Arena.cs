using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Arena : MonoBehaviour
{
    public UnityMovementAI.MovementAIRigidbody[] obstacles;

    public float rotationSpeed;
    public Transform[] rotatingObjects;

    public void Update()
    {
        foreach (Transform t in rotatingObjects)
        {
            t.Rotate(new Vector3(0.0f, 0.0f, 1.0f * rotationSpeed));
        }
    }
}
