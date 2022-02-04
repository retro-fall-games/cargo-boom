using System.Collections.Generic;
using UnityEngine;

public static class DebugEx
{
  public static void LogKeys<K, V>(this Dictionary<K, V> dictionary)
  {
    foreach (KeyValuePair<K, V> kvp in dictionary)
      Debug.Log($"Key = {kvp.Key}");
  }

  public static void LogKeyValues<K, V>(this Dictionary<K, V> dictionary)
  {
    foreach (KeyValuePair<K, V> kvp in dictionary)
      Debug.Log($"Key = {kvp.Key} + Value = {kvp.Value}");
  }

  public static void LogValues<K, V>(this Dictionary<K, V> dictionary)
  {
    foreach (KeyValuePair<K, V> kvp in dictionary)
      Debug.Log($"Value = {kvp.Value}");
  }

  public static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
  {
    float angle = 0f;
    Quaternion rot = Quaternion.LookRotation(forward, up);
    Vector3 lastPoint = Vector3.zero;
    Vector3 thisPoint = Vector3.zero;

    for (int i = 0; i < segments + 1; i++)
    {
      thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
      thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

      if (i > 0)
      {
        Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
      }

      lastPoint = thisPoint;
      angle += 360f / segments;
    }
  }

  public static void DrawCone(Vector3 pos, Vector3 right, float angle, float radius, float coneDirection = 180)
  {
    float rayRange = radius;
    float halfFOV = angle / 2.0f;

    Quaternion upRayRotation = Quaternion.AngleAxis(-halfFOV + coneDirection, Vector3.forward);
    Quaternion downRayRotation = Quaternion.AngleAxis(halfFOV + coneDirection, Vector3.forward);

    Vector3 upRayDirection = upRayRotation * right * rayRange;
    Vector3 downRayDirection = downRayRotation * right * rayRange;

    Gizmos.DrawRay(pos, upRayDirection);
    Gizmos.DrawRay(pos, downRayDirection);
    Gizmos.DrawLine(pos + downRayDirection, pos + upRayDirection);
  }
}
