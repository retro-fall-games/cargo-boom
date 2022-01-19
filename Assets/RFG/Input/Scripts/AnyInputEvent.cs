using UnityEngine;
using UnityEngine.Events;

namespace RFG
{
  [AddComponentMenu("RFG/Input/Any Input Event")]
  public class AnyInputEvent : MonoBehaviour
  {
    [field: SerializeField] private bool Once { get; set; }
    [field: SerializeField] private bool DisableEscape { get; set; } = false;
    [field: SerializeField] private UnityEvent OnInput;

    private bool _hasInvoked;

    private void Awake()
    {
      _hasInvoked = false;
    }

    private void Update()
    {
      if (Once && _hasInvoked)
        return;

      if (InputEx.HasAnyInput())
      {
        if (DisableEscape && InputEx.WasEscapePressedThisFrame())
        {
          return;
        }
        _hasInvoked = true;
        OnInput?.Invoke();
      }
    }
  }
}