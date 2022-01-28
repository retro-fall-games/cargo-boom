using UnityEngine;

namespace RFG.ScrollingShooter
{
  [AddComponentMenu("RFG/Scrolling Shooter/Character/AI Behaviours/AI Aggro")]
  public class AIAggroBehaviour : MonoBehaviour
  {
    private CharacterController2D _controller;
    private Aggro _aggro;

    private void Awake()
    {
      _controller = GetComponent<CharacterController2D>();
      _aggro = GetComponent<Aggro>();
    }

    private void LateUpdate()
    {
      if (_aggro.target2 != null)
      {
        _controller.RotateTowards(_aggro.target2);
      }
    }
  }
}