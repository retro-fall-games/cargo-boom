using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Knockback Data", menuName = "RFG/Platformer/Interactions/Knockback")]
  public class KnockbackData : ScriptableObject
  {
    [Header("Self")]
    public float SelfDamage;
    public Vector2 SelfVelocity;
    public float SelfThreshold = 0.5f;
    public bool SelfAffectRigidBody2D = false;

    [Header("Other")]
    public float OtherDamage;
    public Vector2 OtherVelocity;
    public float OtherThreshold = 0.5f;
    public bool OtherAffectRigidBody2D = false;

    [Header("Settings")]
    public LayerMask LayerMask;
    public string[] Tags;


    [Header("Effects")]
    public string[] Effects;

    public Vector2 GetSelfVelocity(Vector2 target1, Vector2 target2)
    {
      Vector2 dir = (target1 - target2).normalized;
      if (dir.x > -SelfThreshold && dir.x < SelfThreshold)
      {
        dir.x = SelfThreshold;
      }
      if (dir.y > -SelfThreshold && dir.y < SelfThreshold)
      {
        dir.y = SelfThreshold;
      }
      return dir * SelfVelocity;
    }

    public Vector2 GetOtherVelocity(Vector2 target1, Vector2 target2)
    {
      Vector2 dir = (target1 - target2).normalized;
      if (dir.x > -OtherThreshold && dir.x < OtherThreshold)
      {
        dir.x = OtherThreshold;
      }
      if (dir.y > -OtherThreshold && dir.y < OtherThreshold)
      {
        dir.y = OtherThreshold;
      }
      return dir * OtherVelocity;
    }
  }
}