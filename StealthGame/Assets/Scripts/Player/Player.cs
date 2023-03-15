using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityMovementAI;

public class Player : MonoBehaviour
{
    [HideInInspector] public int teamIndex = -1; // set by game mode manager
    [HideInInspector] public bool alive = true;

    private PlayerInput _playerInput; // Component on player prefab; Make sure it uses c# events
    private PlayerInputActions _actionMap; // Asset that defines button to action mappings; must have the asset generate a c# class

    private Transform _transform;
    private Rigidbody2D _rb;
    private MovementAIRigidbody _aiRb;
    [SerializeField] private GameObject _sensor;

    private Vector2 _moveVec = new Vector2(0, 0);

    [SerializeField] private float _speed = 2.0f; // units per second

    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Gradient colorGradient;
    public Color color;

    [SerializeField] private PlayerKnife _knife;
    [SerializeField] private GameObject _knifeParentObject;

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
        // Color gets set by game mode manager when added to a a team
        //RandomizeColor();

        NPCSpawner.S.RandomizePosition(_transform);
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
        else if (context.action.name == _actionMap.Player.Stab.name)
        {
            OnStab(context);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _moveVec = context.ReadValue<Vector2>();
    }

    private void OnStab(InputAction.CallbackContext context)
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

    #endregion

    public void OnStabbed()
    {
        StartCoroutine(RespawnCo());
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
        NPCSpawner.S.RandomizePosition(_transform);
    }

    public void RandomizeColor()
    {
        color = colorGradient.Evaluate(Random.Range(0f, 1f));
        _spriteRenderer.color = color; // tints sprite, will only really work if sprite is white to begin with
    }

    public void SetColor(Color c)
    {
        color = c;
        _spriteRenderer.color = c;
    }
}
