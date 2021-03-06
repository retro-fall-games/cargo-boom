using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.SceneGraph
{
  [AddComponentMenu("RFG/Scene Graph/Scene Bounds")]
  public class SceneBounds : MonoBehaviour
  {
    public Bounds Bounds;

#if UNITY_EDITOR
    [ButtonMethod]
    private void CopyFromSceneGraph()
    {
      Scene _scene = SceneGraphManager.Instance.CurrentScene;
      if (_scene == null)
      {
        return;
      }
      Bounds = _scene.bounds;
      EditorUtility.SetDirty(gameObject);
    }

    [ButtonMethod]
    private void CopyFromSelection()
    {
      Scene _scene = SceneGraphManager.Instance.CurrentScene;
      if (_scene == null)
      {
        return;
      }
      PolygonCollider2D collider = Selection.activeGameObject.GetComponent<PolygonCollider2D>();
      if (collider != null)
      {
        _scene.bounds.min = new Vector3(collider.points[0].x, collider.points[0].y, 0);
        _scene.bounds.max = new Vector3(collider.points[2].x, collider.points[2].y, 0);
      }
      EditorUtility.SetDirty(gameObject);
    }

    [ButtonMethod]
    private void GeneratePolygonCollider2DToSelection()
    {
      PolygonCollider2D collider = Selection.activeGameObject.AddComponent<PolygonCollider2D>();
      Vector2[] points = new Vector2[]
      {
        new Vector2(Bounds.min.x, Bounds.min.y),
        new Vector2(Bounds.min.x, Bounds.max.y),
        new Vector2(Bounds.max.x, Bounds.max.y),
        new Vector2(Bounds.max.x, Bounds.min.y),
      };
      collider.SetPath(0, points);
      EditorUtility.SetDirty(Selection.activeGameObject);
    }

    private void OnDrawGizmos()
    {
      var b = Bounds;
      var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
      var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
      var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
      var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

      Gizmos.DrawLine(p1, p2);
      Gizmos.DrawLine(p2, p3);
      Gizmos.DrawLine(p3, p4);
      Gizmos.DrawLine(p4, p1);

      // top
      var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
      var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
      var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
      var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

      Gizmos.DrawLine(p5, p6);
      Gizmos.DrawLine(p6, p7);
      Gizmos.DrawLine(p7, p8);
      Gizmos.DrawLine(p8, p5);

      // sides
      Gizmos.DrawLine(p1, p5);
      Gizmos.DrawLine(p2, p6);
      Gizmos.DrawLine(p3, p7);
      Gizmos.DrawLine(p4, p8);
    }
#endif

  }
}