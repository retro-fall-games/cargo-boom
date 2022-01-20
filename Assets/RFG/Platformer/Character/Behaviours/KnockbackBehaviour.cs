using UnityEngine;
using RFG.Character;

namespace RFG.Platformer
{
  [AddComponentMenu("RFG/Platformer/Character/Behaviours/Knockback")]
  public class KnockbackBehaviour : MonoBehaviour
  {
    public State ChangeCharacterState;
    public State ChangeMovementState;

    private Character _character;
    private CharacterController2D _controller;
    private HealthBehaviour _health;

    private void Awake()
    {
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _health = GetComponent<HealthBehaviour>();
    }

    private void ChangeState(GameObject other)
    {
      if (_character != null)
      {
        if (ChangeCharacterState != null)
        {
          _character.CharacterState.ChangeState(ChangeCharacterState.GetType());
        }
        if (ChangeMovementState != null)
        {
          _character.MovementState.ChangeState(ChangeMovementState.GetType());
        }
      }
    }

    private void AddVelocityCharacterController2D(GameObject other, KnockbackData KnockbackData)
    {
      if (_controller != null)
      {
        Vector2 velocity = KnockbackData.GetOtherVelocity(other.transform.position, transform.position);
        if (_controller != null && _controller.Parameters.Weight > 0)
        {
          velocity /= _controller.Parameters.Weight;
        }
        _controller.SetForce(velocity);
      }
    }

    private void AddDamage(GameObject other, KnockbackData KnockbackData)
    {
      if (KnockbackData.OtherDamage > 0f)
      {
        if (_health != null)
        {
          _health.TakeDamage(KnockbackData.OtherDamage);
        }
      }
    }

    private void PerformKnockback(GameObject other)
    {
      KnockbackData KnockbackData = null;

      Knockback knockback = other.GetComponent<Knockback>();
      if (knockback != null)
      {
        KnockbackData = knockback.KnockbackData;
      }

      if (KnockbackData.LayerMask.Contains(other.layer) || (KnockbackData.Tags != null && other.CompareTags(KnockbackData.Tags)))
      {
        ChangeState(other);
        AddVelocityCharacterController2D(other, KnockbackData);
        AddDamage(other, KnockbackData);
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