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

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _stabSound;

    public GameObject bloodPrefab;

    private void Start()
    {
        _restPos = _restTrans.localPosition;
        _stabPos = _stabTrans.localPosition;
    }

    public void Stab(Vector2 dir)
    {
        // Knife occaisionally goes in opposite direction when you very lightly move the stick
        _knifeParent.rotation = Quaternion.EulerAngles(0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x));
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
                _audioSource.PlayOneShot(_stabSound);
                Instantiate(bloodPrefab, obj.transform.position, Quaternion.identity);
                victim.OnStabbed();
                GameModeManager.S.OnPlayerKilledPlayer(_player, victim);
            }
            else if (collision.gameObject.CompareTag("NPC"))
            {
                _audioSource.PlayOneShot(_stabSound);
                Instantiate(bloodPrefab, obj.transform.position, Quaternion.identity);
                NPCSpawner.S.RemoveNPC(obj.GetComponent<MovementAIRigidbody>());
                Destroy(obj);
                GameModeManager.S.OnPlayerKilledNPC(_player);
                //NPCSpawner.S.TryToCreateObject();
            }
        }
    }
}
