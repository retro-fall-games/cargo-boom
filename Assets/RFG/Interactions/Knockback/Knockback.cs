using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Knockback")]
  public class Knockback : MonoBehaviour
  {
    public KnockbackData KnockbackData;

    private Rigidbody2D rb;

    private void Awake()
    {
      rb = GetComponent<Rigidbody2D>();
    }

    private void AddSelfVelocityRigidBody2D(GameObject other)
    {
      if (rb != null)
      {
        Vector2 velocity = KnockbackData.GetSelfVelocity(transform.position, other.transform.position);
        rb.AddForce(velocity);
      }
    }

    private void AddOtherVelocityRigidBody2D(GameObject other)
    {
      Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
      if (otherRb != null)
      {
        Vector2 velocity = KnockbackData.GetOtherVelocity(other.transform.position, transform.position);
        otherRb.AddForce(velocity);
      }
    }

    private void PerformKnockback(GameObject other)
    {
      if (KnockbackData == null)
      {
        return;
      }
      if (KnockbackData.LayerMask.Contains(other.layer) || (KnockbackData.Tags != null && other.CompareTags(KnockbackData.Tags)))
      {
        transform.SpawnFromPool(KnockbackData.Effects);
        if (KnockbackData.SelfAffectRigidBody2D)
        {
          AddSelfVelocityRigidBody2D(other);
        }
        if (KnockbackData.OtherAffectRigidBody2D)
        {
          AddOtherVelocityRigidBody2D(other);
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
      PerformKnockback(col.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
      PerformKnockback(other);
    }


  }
}