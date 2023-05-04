using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityMovementAI;
using Random = UnityEngine.Random;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private Transform _npcPrefab;

    // Actual absolute scale is randomly set in this range, not relative to prefabs scale values
    public float npcMinScale = 0.5f;
    public float npcMaxScale = 0.7f;

    public float respawnDelay = 1.0f;

    public bool randomizeOrientation = true;
    public bool randomizeColor = true;
    //public Gradient colorGradient;

    public float boundaryPadding = 1.0f;
    public float spaceBetweenObjects = 1.0f;

    public MovementAIRigidbody[] obstacles;


    private Vector3 bottomLeft;
    private Vector3 widthHeight;
    private bool isObj3D;

    private Transform _npcParentTransform;

    [System.NonSerialized]
    public List<MovementAIRigidbody> npcs = new List<MovementAIRigidbody>();

    void Start()
    {
        _npcParentTransform = transform;
        
        MovementAIRigidbody rb = _npcPrefab.GetComponent<MovementAIRigidbody>();
        // Manually set up the MovementAIRigidbody since the given obj can be a prefab
        rb.SetUp();
        isObj3D = rb.is3D;

        // Find the size of the map
        float distAway = Camera.main.WorldToViewportPoint(Vector3.zero).z;

        bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distAway));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distAway));
        widthHeight = topRight - bottomLeft;
    }

    public void SpawnNPCs(int num = 100) // not all of these will for sure get spawned, it will attempt 100 spawns, 10 times each max
    {
        for (int i = 0; i < num; i++)
        {
            /* Try to place the objects multiple times before giving up */
            for (int j = 0; j < 10; j++)
            {
                if (TryToCreateObject())
                {
                    break;
                }
            }
        }
    }

    public void DestroyNPCs(Player playerAlive)
    {
        foreach (MovementAIRigidbody npc in npcs)
        {
            StartCoroutine(DestroyNPCWithDelay(playerAlive, npc));
        }
        npcs.Clear();
    }

    IEnumerator DestroyNPCWithDelay(Player playerAlive, MovementAIRigidbody npc)
    {
        float rand = Random.Range(0, GameModeManager.S.uiManager.timeBeforeEndScreen / 3);
        yield return new WaitForSeconds(rand);
        playerAlive.PlayExplosionNPC(npc.transform.position, npc.GetComponent<SpriteRenderer>().color);
        Destroy(npc.gameObject);
    }

    public bool TryToCreateObject()
    {
        float size = Random.Range(npcMinScale, npcMaxScale);
        float halfSize = size / 2f;
        Vector3 pos = GetRandomPos(halfSize, size);

        if (CanPlaceObject(halfSize, pos))
        {
            Transform t = Instantiate(_npcPrefab, pos, Quaternion.identity, _npcParentTransform) as Transform;

            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if (sr) SetColorAndSprite(sr);

            if (isObj3D)
            {
                t.localScale = new Vector3(size, _npcPrefab.localScale.y, size);
            }
            else
            {
                t.localScale = new Vector3(size, size, _npcPrefab.localScale.z);
            }

            if (randomizeOrientation)
            {
                Vector3 euler = transform.eulerAngles;
                if (isObj3D)
                {
                    euler.y = Random.Range(0f, 360f);
                }
                else
                {
                    euler.z = Random.Range(0f, 360f);
                }

                transform.eulerAngles = euler;
            }

            npcs.Add(t.GetComponent<MovementAIRigidbody>());

            return true;
        }

        return false;
    }

    public Vector3 GetRandomPos(float halfSize, float size)
    {
        Vector3 pos = new Vector3();
        pos.x = bottomLeft.x + Random.Range(boundaryPadding + halfSize, widthHeight.x - boundaryPadding - halfSize);

        if (isObj3D)
        {
            pos.z = bottomLeft.z + Random.Range(boundaryPadding + halfSize, widthHeight.z - boundaryPadding - halfSize);
        }
        else
        {
            pos.y = bottomLeft.y + Random.Range(boundaryPadding + halfSize, widthHeight.y - boundaryPadding - halfSize);
        }

        return pos;
    }

    bool CanPlaceObject(float halfSize, Vector3 pos)
    {
        // Make sure it does not overlap with any thing to avoid
        for (int i = 0; i < obstacles.Length; i++)
        {
            float dist = Vector3.Distance(obstacles[i].Position, pos);

            if (dist < halfSize + obstacles[i].Radius)
            {
                return false;
            }
        }

        // Make sure it does not overlap with any existing object
        foreach (MovementAIRigidbody npc in npcs)
        {
            float dist = Vector3.Distance(npc.Position, pos);

            if (dist < npc.Radius + spaceBetweenObjects + halfSize)
            {
                return false;
            }
        }

        return true;
    }

    public void RandomizePosition(Transform trans)
    {
        // assumes players are the same 1 unit by 1 unit circles with equivalent x and y scaling
        Vector3 pos = GetRandomPos(trans.localScale.x / 2, trans.localScale.x);

        int i = 0;
        while (!CanPlaceObject(trans.localScale.x / 2, pos))
        {
            if (i > 10) break;
            i++;
            pos = GetRandomPos(trans.localScale.x / 2, trans.localScale.x);
        }

        trans.position = pos;
    }

    public void RemoveNPC(MovementAIRigidbody npc)
    {
        npcs.Remove(npc);
    }

    public void RemoveNPCAndRespawn(MovementAIRigidbody npc)
    {
        npcs.Remove(npc);
        StartCoroutine(RespawnNPC());
    }

    private IEnumerator RespawnNPC()
    {
        yield return new WaitForSeconds(Random.Range(respawnDelay * 0.5f, respawnDelay * 1.5f));

        if (GameModeManager.S.gameState == GameModeManager.GameState.playing)
        {
            float size = Random.Range(npcMinScale, npcMaxScale);
            float halfSize = size / 2f;
            Vector3 pos = GetRandomPos(halfSize, size);

            Transform t = Instantiate(_npcPrefab, pos, Quaternion.identity, _npcParentTransform) as Transform;

            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if (sr) SetColorAndSprite(sr);

            npcs.Add(t.GetComponent<MovementAIRigidbody>());
        }
    }

    public void RandomizeColor(SpriteRenderer sr)
    {
        //Color randColor = colorGradient.Evaluate(Random.Range(0f, 1f));
        //sr.color = randColor; // tints sprite, will only really work if sprite is white to begin with
    }

    public void SetColorAndSprite(SpriteRenderer sr)
    {
        List<Color> colors = GameModeManager.S.colorManager.currentColorProfile.npcColors;
        List<Sprite> sprites = GameModeManager.S.colorManager.currentColorProfile.npcSprites;
        int randColor = Random.Range(0, colors.Count);
        int randSprite = Random.Range(0, sprites.Count);
        sr.color = colors[randColor];
        sr.sprite = sprites[randSprite];
    }
}