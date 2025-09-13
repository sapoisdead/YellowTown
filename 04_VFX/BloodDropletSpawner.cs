using System.Collections.Generic;
using UnityEngine;

public class BloodDropletSpawner : MonoBehaviour
{
    ParticleSystem _particleSystem;

    private readonly List<ParticleCollisionEvent> collisionEvents = new();

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = _particleSystem.GetCollisionEvents(other, collisionEvents);

        if (numCollisionEvents > 0) // Verifica che ci sia almeno un evento di collisione
        {
            Vector2 collisionPoint = collisionEvents[0].intersection;
            EventManager.Instance.TriggerBloodDropletFXEvent(collisionPoint);
        }
    }
}
