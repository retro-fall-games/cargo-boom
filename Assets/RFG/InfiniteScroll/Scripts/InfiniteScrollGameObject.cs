using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Infinite Scroll/Infinite Scroll Game Object")]
  public class InfiniteScrollGameObject : MonoBehaviour
  {
    [field: SerializeField] private bool InfiniteHorizontal { get; set; }
    [field: SerializeField] private bool InfiniteVertical { get; set; }
    [field: SerializeField] private float Choke { get; set; } = 16f;
    [field: SerializeField] private float Width { get; set; }
    [field: SerializeField] private float Height { get; set; }

    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;
    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;
    private Vector2 _screenBounds;

    private void Awake()
    {
      Camera mainCamera = Camera.main;
      _cameraTransform = mainCamera.transform;
      _lastCameraPosition = _cameraTransform.position;
      _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    private void LateUpdate()
    {
      Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;

      if (deltaMovement.Equals(Vector3.zero))
        return;

      if (InfiniteHorizontal)
      {
        if (deltaMovement.x > 0 && _cameraTransform.position.x + _screenBounds.x + Choke > transform.position.x + Width)
        {
          MoveHorizontal(transform.position.x + Width);
        }
        else if (deltaMovement.x < 0 && _cameraTransform.position.x - _screenBounds.x - Choke < transform.position.x - Width)
        {
          MoveHorizontal(transform.position.x - Width);
        }
      }

      if (InfiniteVertical)
      {
        if (deltaMovement.y > 0 && _cameraTransform.position.y + _screenBounds.y + Choke > transform.position.y + Height)
        {
          MoveVertical(transform.position.y + Height);
        }
        else if (deltaMovement.y < 0 && _cameraTransform.position.y - _screenBounds.y - Choke < transform.position.y - Height)
        {
          MoveVertical(transform.position.y - Height);
        }
      }

      _lastCameraPosition = _cameraTransform.position;
    }

    private void MoveHorizontal(float x)
    {
      transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void MoveVertical(float y)
    {
      transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

  }
}
