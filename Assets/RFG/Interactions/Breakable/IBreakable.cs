using UnityEngine;

namespace RFG
{
  interface IBreakable
  {
    void Hit(int damage, Vector3 point, Vector3 normal);
    void Break(Vector3 point, Vector3 normal);
  }
}