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
  }
}