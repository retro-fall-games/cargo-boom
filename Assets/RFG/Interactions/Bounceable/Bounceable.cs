using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  public enum BounceAffectType { RigidBody2D, IPhysics2D };

  [AddComponentMenu("RFG/Interactions/Bounceable")]
  public class Bounceable : MonoBehaviour
  {
    [field: SerializeField] private LayerMask LayerMask { get; set; }
    [field: SerializeField] private List<string> BounceEffects { get; set; }
    [field: SerializeField] private string BounceClip { get; set; }
    [field: SerializeField] private string BounceClipLayer { get; set; } = "Base Layer";
    [field: SerializeField] private Vector2 BounceVelocity { get; set; }
    [field: SerializeField] private BounceAffectType BounceAffectType { get; set; }

    private Animator _animator;

    private void Awake()
    {
      _animator = GetComponent<Animator>();
    }

    private void PerformBounce(GameObject other)
    {
      if (LayerMask.Contains(other.layer))
      {
        transform.SpawnFromPool(BounceEffects.ToArray());

        int hash = Animator.StringToHash(BounceClip);
        int layerIndex = _animator.GetLayerIndex(BounceClipLayer);
        if (_animator.HasState(layerIndex, hash))
        {
          _animator.Play(BounceClip);
        }

        switch (BounceAffectType)
        {
          case BounceAffectType.RigidBody2D:
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
              rb.velocity = Vector3.zero;
              rb.angularVelocity = 0;
              rb.AddForce(BounceVelocity);
            }
            break;
          case BounceAffectType.IPhysics2D:
            IPhysics2D ip = other.GetComponent(typeof(IPhysics2D)) as IPhysics2D;
            if (ip != null)
            {
              ip.SetForce(BounceVelocity);
            }
            break;
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      PerformBounce(col.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
      PerformBounce(other);
    }
  }
}