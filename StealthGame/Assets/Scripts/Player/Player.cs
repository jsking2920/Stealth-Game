using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityMovementAI;

public class Player : MonoBehaviour
{
    public float _maxVelocity = 3.5f;
    [SerializeField] private float _maxAcceleration = 2.25f;
    //[SerializeField] private Gradient colorGradient;
    public bool canStab = true; 

    [SerializeField] private GameObject _sensor;
    [SerializeField] private PlayerKnife _knife;
    [SerializeField] private GameObject _knifeParentObject;

    [SerializeField] private GameObject explosionPrefab;

    public int teamIndex = -1; // set by game mode manager
    public int playerIndex = -1;
    [HideInInspector] public bool alive = true;
    public int lives = 2; // only  used in game modes with finite lives

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
        else if (context.action.name == _actionMap.Player.A.name)
        {
            if (GameModeManager.S.gameState == GameModeManager.GameState.playing)
            {
                GameModeManager.S.uiManager.AnimateScore(teamIndex);
            }
        }
        else if (context.action.name == _actionMap.Player.Pause.name && context.performed)
        {
            if (GameModeManager.S.gameState == GameModeManager.GameState.joining)
            {
                GameModeManager.S.StartGame();
            }
            else if (GameModeManager.S.gameState == GameModeManager.GameState.playing || GameModeManager.S.gameState == GameModeManager.GameState.paused)
            {
                GameModeManager.S.TogglePause();
            }
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
        PlayExplosionSelf();
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
        AudioManager.S.PlayExplosion();

        // Rumble
        Gamepad gamepad = _playerInput.GetDevice<Gamepad>();
        if (gamepad != null)
        {
            StartCoroutine(RumbleCo(gamepad));
        }

        StartCoroutine(GameModeManager.S.uiManager.FlashScoreCo(teamIndex));
    }

    private IEnumerator RumbleCo(Gamepad g)
    {
        g.SetMotorSpeeds(0.6f, 0.6f);
        yield return new WaitForSeconds(0.8f);
        g.SetMotorSpeeds(0.0f, 0.0f);
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
        GameModeManager.S.OnRespawn(this);
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

    private void PlayExplosionSelf()
    {
        ParticleSystem ps = Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = _spriteRenderer.color;
        ps.Play();
        Destroy(ps.gameObject, main.duration + 1.0f);
    }

    public void PlayExplosionNPC(Vector3 pos, Color col)
    {
        ParticleSystem ps = Instantiate(explosionPrefab, pos, Quaternion.identity).GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = col;
        ps.Play();
        Destroy(ps.gameObject, main.duration + 1.0f);
    }
}
