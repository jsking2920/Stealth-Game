using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityMovementAI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public float _maxVelocity = 3.5f;
    [SerializeField] private float _maxAcceleration = 2.25f;
    //[SerializeField] private Gradient colorGradient;
    public bool canStab = true; 

    [SerializeField] private GameObject _sensor;
    [SerializeField] private PlayerKnife _knife;
    [SerializeField] private GameObject _knifeParentObject;


    public int teamIndex = -1; // set by game mode manager
    [HideInInspector] public bool alive = true;
    public int lives = 3; // only  used in game modes with finite lives
    [HideInInspector] public Color color;

    private Vector2 _moveVec = new Vector2(0, 0); // Should be normalized to [0, 1]

    private PlayerInput _playerInput; // Component on player prefab; Make sure it uses c# events
    private PlayerInputActions _actionMap; // Asset that defines button to action mappings; must have the asset generate a c# class

    private Transform _transform;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private MovementAIRigidbody _aiRb;

    // Called in game mode manager when added to a team
    public void Setup()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _aiRb = GetComponent<MovementAIRigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _actionMap = new PlayerInputActions();

        _playerInput.onActionTriggered += Input_onActionTriggered;

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        _rb.velocity += _moveVec * _maxAcceleration;
        _rb.velocity = Vector2.ClampMagnitude(_rb.velocity, _maxVelocity);
        _transform.right = _rb.velocity.normalized;
    }

    #region Input Handling

    private void Input_onActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == _actionMap.Player.Move.name)
        {
            OnMove(context);
        }
        else if (context.action.name == _actionMap.Player.Stab.name)
        {
            OnStab(context);
        }
        else if (context.action.name == _actionMap.Player.Pause.name)
        {
            GameModeManager.S.TogglePause();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveVec = context.ReadValue<Vector2>();
    }

    private void OnStab(InputAction.CallbackContext context)
    {
        if (canStab)
        {
            if (context.canceled)
            {
                _knifeParentObject.SetActive(false);
            }
            else if (context.started)
            {
                _knifeParentObject.SetActive(true);
            }
            else
            {
                _knife.Stab(context.ReadValue<Vector2>());
            } 
        }
    }

    #endregion

    public void OnStabbed(Player killer)
    {
        if (lives > 0)
            StartCoroutine(RespawnCo());
        else
        {
            alive = false;
            _spriteRenderer.enabled = false;
            _aiRb.enabled = false;
            _sensor.SetActive(false);
            transform.position = new Vector3(1000, 1000, 1000);
        }
    }
    
    private IEnumerator RespawnCo()
    {
        alive = false;
        _spriteRenderer.enabled = false;
        _aiRb.enabled = false;
        _sensor.SetActive(false);
        transform.position = new Vector3(1000, 1000, 1000);

        yield return new WaitForSeconds(GameModeManager.S.playerRespawnTime);

        alive = true;
        _spriteRenderer.enabled = true;
        _aiRb.enabled = true;
        _sensor.SetActive(true);
        GameModeManager.S.SetSpawnPosition(_transform);
    }

    public void RandomizeColor()
    {
        //color = colorGradient.Evaluate(Random.Range(0f, 1f));
        //_spriteRenderer.color = color; // tints sprite, will only really work if sprite is white to begin with
    }

    public void SetColor(Color c)
    {
        _spriteRenderer.color = c;
    }
}
