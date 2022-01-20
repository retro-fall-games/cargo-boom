using UnityEngine;

namespace RFG.Platformer
{
  [AddComponentMenu("RFG/Platformer/Interactions/Physics/Gravity Point")]
  public class GravityPoint : MonoBehaviour
  {
    public float DistanceOfEffect;

    protected virtual void OnDrawGizmos()
    {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(transform.position, DistanceOfEffect);
    }
  }
}
