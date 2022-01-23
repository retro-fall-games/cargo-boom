using UnityEngine;

namespace RFG.ScrollingShooter
{
  public class AIAjent
  {
    public Transform transform;
    public Character character;
    public CharacterController2D controller;
    public StateCharacterContext characterContext;
    public Aggro aggro;
    public Animator animator;
    public Tween movementPath;
    public AIBrainBehaviour aiState;
    public bool JustRotated = false;
    public float LastTimeRotated = 0f;
    public float RotateSpeed = 0f;
  }
}