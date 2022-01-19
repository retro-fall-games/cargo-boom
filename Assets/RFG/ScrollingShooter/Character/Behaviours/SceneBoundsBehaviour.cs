using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RFG.ScrollingShooter
{
  [AddComponentMenu("RFG/Scrolling Shooter/Character/Behaviours/Scene Bounds")]
  public class SceneBoundsBehaviour : MonoBehaviour
  {
    public enum BoundsBehaviour { Nothing, Constrain, Kill }

    [Header("Bounds Behaviour")]
    public BoundsBehaviour Top = BoundsBehaviour.Constrain;
    public BoundsBehaviour Bottom = BoundsBehaviour.Kill;
    public BoundsBehaviour Left = BoundsBehaviour.Constrain;
    public BoundsBehaviour Right = BoundsBehaviour.Constrain;
    public Bounds Bounds = new Bounds(Vector3.zero, Vector3.one * 10);

    private Character _character;
    private BoxCollider2D _boxCollider;

    private void Awake()
    {
      _character = GetComponent<Character>();
      _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
      if (_character.CharacterState.CurrentStateType != typeof(AliveState))
        return;

      HandleLevelBounds(_character);
    }

    public void HandleLevelBounds(Character _character)
    {
      if (Bounds.size != Vector3.zero)
      {

        var colliderSize = new Vector2(
          _boxCollider.size.x * Mathf.Abs(transform.localScale.x),
          _boxCollider.size.y * Mathf.Abs(transform.localScale.y)
        ) / 2;

        if (Top != BoundsBehaviour.Nothing && transform.position.y + colliderSize.y > Bounds.max.y)
        {
          ApplyBoundsBehaviour(Top, new Vector2(transform.position.x, Bounds.max.y - colliderSize.y));
        }

        if (Bottom != BoundsBehaviour.Nothing && transform.position.y - colliderSize.y < Bounds.min.y)
        {
          ApplyBoundsBehaviour(Bottom, new Vector2(transform.position.x, Bounds.min.y + colliderSize.y));
        }

        if (Right != BoundsBehaviour.Nothing && transform.position.x + colliderSize.x > Bounds.max.x)
        {
          ApplyBoundsBehaviour(Right, new Vector2(Bounds.max.x - colliderSize.x, transform.position.y));
        }

        if (Left != BoundsBehaviour.Nothing && transform.position.x - colliderSize.x < Bounds.min.x)
        {
          ApplyBoundsBehaviour(Left, new Vector2(Bounds.min.x + colliderSize.x, transform.position.y));
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
