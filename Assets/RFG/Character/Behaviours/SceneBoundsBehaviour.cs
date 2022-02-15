using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RFG.SceneGraph;

namespace RFG.Character
{
  [AddComponentMenu("RFG/Character/Behaviours/Scene Bounds")]
  public class SceneBoundsBehaviour : MonoBehaviour
  {
    public enum BoundsBehaviour { Nothing, Constrain, Kill }

    [Header("Bounds Behaviour")]
    public BoundsBehaviour Top = BoundsBehaviour.Constrain;
    public BoundsBehaviour Bottom = BoundsBehaviour.Kill;
    public BoundsBehaviour Left = BoundsBehaviour.Constrain;
    public BoundsBehaviour Right = BoundsBehaviour.Constrain;
    public SceneBounds SceneBounds;

    private Character _character;
    private BoxCollider2D _boxCollider;

    private void Awake()
    {
      _character = GetComponent<Character>();
      _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
      HandleLevelBounds();
    }

    public void ChangeTop(string changeTo)
    {
      switch (changeTo)
      {
        case "Nothing":
          Top = BoundsBehaviour.Nothing;
          break;
        case "Kill":
          Top = BoundsBehaviour.Kill;
          break;
        case "Constrain":
          Top = BoundsBehaviour.Constrain;
          break;
      }
    }

    public void ChangeBottom(string changeTo)
    {
      switch (changeTo)
      {
        case "Nothing":
          Bottom = BoundsBehaviour.Nothing;
          break;
        case "Kill":
          Bottom = BoundsBehaviour.Kill;
          break;
        case "Constrain":
          Bottom = BoundsBehaviour.Constrain;
          break;
      }
    }

    public void ChangeLeft(string changeTo)
    {
      switch (changeTo)
      {
        case "Nothing":
          Left = BoundsBehaviour.Nothing;
          break;
        case "Kill":
          Left = BoundsBehaviour.Kill;
          break;
        case "Constrain":
          Left = BoundsBehaviour.Constrain;
          break;
      }
    }

    public void ChangeRight(string changeTo)
    {
      switch (changeTo)
      {
        case "Nothing":
          Right = BoundsBehaviour.Nothing;
          break;
        case "Kill":
          Right = BoundsBehaviour.Kill;
          break;
        case "Constrain":
          Right = BoundsBehaviour.Constrain;
          break;
      }
    }

    private void HandleLevelBounds()
    {
      if (SceneBounds.Bounds.size != Vector3.zero)
      {

        var colliderSize = new Vector2(
          _boxCollider.size.x * Mathf.Abs(transform.localScale.x),
          _boxCollider.size.y * Mathf.Abs(transform.localScale.y)
        ) / 2;

        if (Top != BoundsBehaviour.Nothing && transform.position.y + colliderSize.y > SceneBounds.Bounds.max.y + SceneBounds.transform.position.y)
        {
          ApplyBoundsBehaviour(Top, new Vector2(transform.position.x, SceneBounds.Bounds.max.y + SceneBounds.transform.position.y - colliderSize.y));
        }

        if (Bottom != BoundsBehaviour.Nothing && transform.position.y - colliderSize.y < SceneBounds.Bounds.min.y + SceneBounds.transform.position.y)
        {
          ApplyBoundsBehaviour(Bottom, new Vector2(transform.position.x, SceneBounds.Bounds.min.y + SceneBounds.transform.position.y + colliderSize.y));
        }

        if (Right != BoundsBehaviour.Nothing && transform.position.x + colliderSize.x > SceneBounds.Bounds.max.x + SceneBounds.transform.position.x)
        {
          ApplyBoundsBehaviour(Right, new Vector2(SceneBounds.Bounds.max.x + SceneBounds.transform.position.x - colliderSize.x, transform.position.y));
        }

        if (Left != BoundsBehaviour.Nothing && transform.position.x - colliderSize.x < SceneBounds.Bounds.min.x + SceneBounds.transform.position.x)
        {
          ApplyBoundsBehaviour(Left, new Vector2(SceneBounds.Bounds.min.x + SceneBounds.transform.position.x + colliderSize.x, transform.position.y));
        }
      }
    }

    private void ApplyBoundsBehaviour(BoundsBehaviour Behaviour, Vector2 constrainedPosition)
    {
      if (Behaviour == BoundsBehaviour.Kill)
      {
        _character.Kill();
      }
      _character.transform.position = constrainedPosition;
    }

#if UNITY_EDITOR
    [ButtonMethod]
    private void GeneratePolygonCollider2DToSelection()
    {
      PolygonCollider2D collider = Selection.activeGameObject.AddComponent<PolygonCollider2D>();
      Vector2[] points = new Vector2[]
      {
        new Vector2(SceneBounds.Bounds.min.x, SceneBounds.Bounds.min.y),
        new Vector2(SceneBounds.Bounds.min.x, SceneBounds.Bounds.max.y),
        new Vector2(SceneBounds.Bounds.max.x, SceneBounds.Bounds.max.y),
        new Vector2(SceneBounds.Bounds.max.x, SceneBounds.Bounds.min.y),
      };
      collider.SetPath(0, points);
      EditorUtility.SetDirty(Selection.activeGameObject);
    }

    private void OnDrawGizmos()
    {
      if (SceneBounds == null)
      {
        return;
      }
      var b = SceneBounds.Bounds;
      var p1 = new Vector3(b.min.x + SceneBounds.transform.position.x, b.min.y + SceneBounds.transform.position.y, b.min.z);
      var p2 = new Vector3(b.max.x + SceneBounds.transform.position.x, b.min.y + SceneBounds.transform.position.y, b.min.z);
      var p3 = new Vector3(b.max.x + SceneBounds.transform.position.x, b.min.y + SceneBounds.transform.position.y, b.max.z);
      var p4 = new Vector3(b.min.x + SceneBounds.transform.position.x, b.min.y + SceneBounds.transform.position.y, b.max.z);

      Gizmos.DrawLine(p1, p2);
      Gizmos.DrawLine(p2, p3);
      Gizmos.DrawLine(p3, p4);
      Gizmos.DrawLine(p4, p1);

      // top
      var p5 = new Vector3(b.min.x + SceneBounds.transform.position.x, b.max.y + SceneBounds.transform.position.y, b.min.z);
      var p6 = new Vector3(b.max.x + SceneBounds.transform.position.x, b.max.y + SceneBounds.transform.position.y, b.min.z);
      var p7 = new Vector3(b.max.x + SceneBounds.transform.position.x, b.max.y + SceneBounds.transform.position.y, b.max.z);
      var p8 = new Vector3(b.min.x + SceneBounds.transform.position.x, b.max.y + SceneBounds.transform.position.y, b.max.z);

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
