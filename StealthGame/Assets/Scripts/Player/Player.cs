using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput _playerInput; // Component on player prefab
    private PlayerInputActions _actionMap; // Asset that defines button to action mappings

    private Transform _transform;
    private Rigidbody2D _rb;

    [SerializeField] private float _speed = 2.0f;

    private void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();

        _actionMap = new PlayerInputActions();
        _actionMap.Player.Enable();
        _actionMap.Player.Move.performed += Move;
    }

    private void Move(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>().normalized;
        Vector2 moveVec = _speed * dir;
        //_rb.AddForce(dir * _speed, ForceMode2D.Force);
        _rb.MovePosition(new Vector2(_transform.position.x + moveVec.x, _transform.position.y + moveVec.y));
    }
}
