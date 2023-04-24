using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityMovementAI;

public class PlayerKnife : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _playerObj;

    public Transform _restTrans;
    public Transform _stabTrans;
    private Vector3 _restPos;
    private Vector3 _stabPos;

    [SerializeField] private Transform _knifeParent; // for rotations

    public GameObject bloodPrefab;

    private void Start()
    {
        _restPos = _restTrans.localPosition;
        _stabPos = _stabTrans.localPosition;
    }

    public void Stab(Vector2 dir)
    {
        // Knife occaisionally goes in opposite direction when you very lightly move the stick
        _knifeParent.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x));
        transform.localPosition = Vector3.Lerp(_restPos, _stabPos, dir.magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameModeManager.S.playerInteractionEnabled)
        {
            GameObject obj = collision.gameObject;
            if (obj.CompareTag("Player") && obj != _playerObj)
            {
                Player victim = obj.GetComponent<Player>();
                // Instantiate(bloodPrefab, obj.transform.position, Quaternion.identity);
                Debug.Log(victim.pS.isPlaying);
                victim.pS.Play();
                Debug.Log(victim.pS.isPlaying);
                GameModeManager.S.OnPlayerKilledPlayer(_player, victim);
                victim.OnStabbed(_player);
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                // Instantiate(bloodPrefab, obj.transform.position, Quaternion.identity);
                AudioManager.S.PlayExplosion();
                collision.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                MovementAIRigidbody npc = obj.GetComponent<MovementAIRigidbody>();
                GameModeManager.S.OnPlayerKilledNPC(_player, npc);
            }
        }
    }
}
