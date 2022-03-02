using UnityEngine;

namespace RFG
{
  public static class BoundsEx
  {
    public static bool IsVisibleFrom(this Bounds bounds, Camera camera)
    {
      Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
      return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
  }
}