using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCParticleSystem : MonoBehaviour
{
    private SpriteRenderer sprite;
    private ParticleSystem particles;

    void Start()
    {
        sprite = GetComponentInParent<SpriteRenderer>();
        particles = gameObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        var main = particles.main;
        main.startColor = sprite.color;
    }
}
