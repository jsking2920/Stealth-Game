using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInput _playerInput; // Component on player prefab; Make sure it uses c# events
    private PlayerInputActions _actionMap; // Asset that defines button to action mappings; must have the asset generate a c# class

    private Transform _transform;
    private Rigidbody2D _rb;

    private Vector2 _moveVec = new Vector2(0, 0);

    [SerializeField] private float _speed = 2.0f; // units per second

    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Gradient colorGradient;

    private void Start()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _actionMap = new PlayerInputActions();

        _playerInput.onActionTriggered += Input_onActionTriggered;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        RandomizeColor();
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveVec * _speed; // _moveVec set in OnMove
        _transform.right = _moveVec; // arrow on player sprite faces right, this makes player face in the direction they move
    }

    #region Input Handling

    private void Input_onActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == _actionMap.Player.Move.name)
        {
            OnMove(context);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveVec = context.ReadValue<Vector2>();
    }

    #endregion

    private void RandomizeColor()
    {
        Color randColor = colorGradient.Evaluate(Random.Range(0f, 1f));
        _spriteRenderer.color = randColor; // tints sprite, will only really work if sprite is white to begin with
    }
}
