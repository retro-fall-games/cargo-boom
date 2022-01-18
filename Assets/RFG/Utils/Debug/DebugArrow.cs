using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Utils/Debug/Debug Arrow")]
  public class DebugArrow : MonoBehaviour
  {
    public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowHeadLength = 0.2f, float arrowHeadAngle = 35f)
    {
      Debug.DrawRay(origin, direction, color);
      DrawArrowEnd(false, origin, direction, color, arrowHeadLength, arrowHeadAngle);
    }

    public static void DebugDrawArrow(Vector3 origin, Vector3 direction, Color color, float arrowLength, float arrowHeadLength = 0.20f, float arrowHeadAngle = 35.0f)
    {
      Debug.DrawRay(origin, direction * arrowLength, color);
      DrawArrowEnd(false, origin, direction * arrowLength, color, arrowHeadLength, arrowHeadAngle);
    }

    private static void DrawArrowEnd(bool drawGizmos, Vector3 arrowEndPosition, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 40.0f)
    {
      if (direction == Vector3.zero)
      {
        return;
      }
      Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
      Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
      Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
      Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
      if (drawGizmos)
      {
        Gizmos.color = color;
        Gizmos.DrawRay(arrowEndPosition + direction, right * arrowHeadLength);
        Gizmos.DrawRay(arrowEndPosition + direction, left * arrowHeadLength);
        Gizmos.DrawRay(arrowEndPosition + direction, up * arrowHeadLength);
        Gizmos.DrawRay(arrowEndPosition + direction, down * arrowHeadLength);
      }
      else
      {
        Debug.DrawRay(arrowEndPosition + direction, right * arrowHeadLength, color);
        Debug.DrawRay(arrowEndPosition + direction, left * arrowHeadLength, color);
        Debug.DrawRay(arrowEndPosition + direction, up * arrowHeadLength, color);
        Debug.DrawRay(arrowEndPosition + direction, down * arrowHeadLength, color);
      }
    }

  }
}