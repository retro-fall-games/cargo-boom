using UnityEngine;

namespace RFG
{
  public class MoveWithGameObject : MonoBehaviour
  {
    [field: SerializeField] private GameObject MoveWith { get; set; }
    [field: SerializeField] private bool Horizontal { get; set; }
    [field: SerializeField] private bool Vertical { get; set; }

    private Vector3 _lastPosition;
    private Vector3 _delta;

    private void LateUpdate()
    {
      _delta = MoveWith.transform.position - _lastPosition;
      float x = 0;
      float y = 0;
      if (Horizontal)
      {
        x = _delta.x;
      }
      if (Vertical)
      {
        y = _delta.y;
      }
      transform.position += new Vector3(x, y, 0);
      _lastPosition = MoveWith.transform.position;
    }
  }
}