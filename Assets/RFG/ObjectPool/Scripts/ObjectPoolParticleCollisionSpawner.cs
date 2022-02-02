using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Object Pool/Object Pool Particle Collision Spawner")]
  public class ObjectPoolParticleCollisionSpawner : MonoBehaviour
  {
    [field: SerializeField] private List<string> Tags { get; set; }
    [field: SerializeField] private UnityEvent OnSpawn { get; set; }

    private ParticleSystem part;
    private List<ParticleCollisionEvent> collisionEvents;

    private void OnParticleCollision(GameObject other)
    {
      part.GetCollisionEvents(other, collisionEvents);
      Vector2 pos = collisionEvents[0].intersection;
      pos.SpawnFromPool(new Vector3(0, 0, 0), Tags.ToArray());
    }

  }
}