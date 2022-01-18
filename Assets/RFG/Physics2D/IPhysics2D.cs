using UnityEngine;

namespace RFG
{
  public interface IPhysics2D
  {
    void AddForce(Vector2 force);
    void SetForce(Vector2 force);
  }
}