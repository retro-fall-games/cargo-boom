using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Input/MouseHoverCursor")]
  public class MouseHoverCursor : MonoBehaviour
  {
    [field: SerializeField] private Texture2D CursorTexture { get; set; }
    [field: SerializeField] private CursorMode CursorMode { get; set; } = CursorMode.Auto;
    [field: SerializeField] private Vector2 HotSpot { get; set; } = Vector2.zero;

    private void OnMouseEnter()
    {
      Cursor.SetCursor(CursorTexture, HotSpot, CursorMode);
    }

    private void OnMouseExit()
    {
      Cursor.SetCursor(null, Vector2.zero, CursorMode);
    }
  }
}