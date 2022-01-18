using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Interactions/Physics/Gravity Zone")]
  public class GravityZone : PhysicsVolume2D
  {
    [Range(0, 360)]
    public float GravityDirectionAngle = 180;
    public Vector2 GravityDirectionVector { get { return Math.RotateVector2(Vector2.down, GravityDirectionAngle); } }

    protected virtual void OnDrawGizmosSelected()
    {
      DebugArrow.DebugDrawArrow(this.transform.position, GravityDirectionVector, Color.green, 3f, 0.2f, 35f);
    }
  }
}
