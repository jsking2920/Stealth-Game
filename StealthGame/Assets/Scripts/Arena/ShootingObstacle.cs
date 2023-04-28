using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingObstacle : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
        direction = direction.normalized;
    }

    void Update()
    {
        if (GameModeManager.S.gameState != GameModeManager.GameState.paused)
        {
            _transform.position += direction * speed;
        }
    }
}
