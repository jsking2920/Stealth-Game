using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityMovementAI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    #region Objective Fields
    public enum ObjectiveType
    {
        DieToTeam,
        StandStill,
        OccupyZone,
        KillNpcs,
    }
    
    // other objective handling options: 
    //  - make objective completion scores different per objective
    //  - objectives are never "completed"; achieving them continuously increases score
    
    [Header("Global Objective Fields")]
    public ObjectiveType _objective;
    public bool _objectiveCompleted { get; private set; }
    [SerializeField] private int _objectiveCompletionScore;
    
    [Header("Specific Objective Fields")]
    [SerializeField] private float _standStillMaxTime;
    private float _standStillCurTime;

    [SerializeField] private float _occupyZoneMaxTime;
    [HideInInspector] public float _occupyZoneCurTime;

    [SerializeField] private int _killNpcsMaxCount;
    [HideInInspector] public int _killNpcsCurCount;

    #endregion

    [Header("Other")]
    [SerializeField] private float _speed = 2.0f; // units per second
    [SerializeField] private Gradient colorGradient;

    [SerializeField] private GameObject _sensor;
    [SerializeField] private PlayerKnife _knife;
    [SerializeField] private GameObject _knifeParentObject;


    [HideInInspector] public int teamIndex = -1; // set by game mode manager
    [HideInInspector] public bool alive = true;
    [HideInInspector] public bool eliminated = false;
    [HideInInspector] public Color color;

    private Vector2 _moveVec = new Vector2(0, 0);

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

        _objectiveCompleted = false;
        _standStillCurTime = 0f;
        _occupyZoneCurTime = 0f;
        _killNpcsCurCount = 0;
    }

    private void Update()
    {
        if (!_objectiveCompleted) CheckObjective();
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

    public void CheckObjective()
    {
        // TODO: Eac of these should be a seperate subclass of Player
        switch (_objective)
        {
            case ObjectiveType.StandStill:
                if (_rb.velocity == new Vector2(0, 0))
                {
                    _standStillCurTime += Time.deltaTime;
                }

                if (_standStillCurTime >= _standStillMaxTime)
                {
                    GameModeManager.S.teams[teamIndex].intScore += _objectiveCompletionScore;
                    _objectiveCompleted = true;
                    Debug.Log("Stand Still Objective Completed");
                }
                break;

            case ObjectiveType.KillNpcs:
                if (_killNpcsCurCount >= _killNpcsMaxCount)
                {
                    GameModeManager.S.teams[teamIndex].intScore += _objectiveCompletionScore;
                    _objectiveCompleted = true;
                    Debug.Log("Kill Npcs Objective Completed");
                }
                break;

            case ObjectiveType.OccupyZone:
                if (_occupyZoneCurTime >= _occupyZoneMaxTime)
                {
                    GameModeManager.S.teams[teamIndex].intScore += _objectiveCompletionScore;
                    _objectiveCompleted = true;
                    Debug.Log("Occupy Zone Objective Completed");
                }
                break;
        }
    }
    
    public void OnStabbed(Player killer)
    {
        //check if teammate was killer and if dieToTeam was the objective
        if (!_objectiveCompleted && killer.teamIndex == teamIndex && _objective == ObjectiveType.DieToTeam)
        {
            GameModeManager.S.teams[teamIndex].intScore += _objectiveCompletionScore;
            _objectiveCompleted = true;
            Debug.Log("Die to Team Objective Completed");
        }

        if (GameModeManager.S.doPlayersRespawn && !eliminated)
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
        color = colorGradient.Evaluate(Random.Range(0f, 1f));
        _spriteRenderer.color = color; // tints sprite, will only really work if sprite is white to begin with
    }

    public void SetColor(Color c)
    {
        color = c;
        _spriteRenderer.color = c;
    }
}
