using UnityEngine;

namespace RFG.ScrollingShooter
{
  using BehaviourTree;

  [AddComponentMenu("RFG/Scrolling Shooter/Character/AI Behaviours/AI Brain")]
  public class AIBrainBehaviour : MonoBehaviour, INodeContext
  {
    [Tooltip("The speed / time it takes to rotate and make decisions")]
    public float RotateSpeed;

    [Tooltip("The stun cooldown time to start making decisions again")]
    public float StunCooldownTime = 5f;

    public bool HasAggro { get; set; }

    [HideInInspector]
    public AIAjent Context => _ctx;
    private Transform _transform;
    private Character _character;
    private CharacterController2D _controller;
    private Animator _animator;
    private Tween _movementPath;
    private Aggro _aggro;
    private AIAjent _ctx;

    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _animator = GetComponent<Animator>();
      _movementPath = GetComponent<Tween>();
      _aggro = GetComponent<Aggro>();
    }

    private void Start()
    {
      _ctx = new AIAjent();
      _ctx.transform = _transform;
      _ctx.character = _character;
      _ctx.characterContext = _character.Context;
      _ctx.controller = _controller;
      _ctx.aggro = _aggro;
      _ctx.animator = _animator;
      _ctx.movementPath = _movementPath;
      _ctx.aiState = this;
      _ctx.RotateSpeed = RotateSpeed;
    }

    private void OnAggroChange(bool aggro)
    {
      HasAggro = aggro;
    }

    private void OnEnable()
    {
      if (_aggro != null)
      {
        _aggro.OnAggroChange += OnAggroChange;
      }
    }

    private void OnDisable()
    {
      if (_aggro != null)
      {
        _aggro.OnAggroChange -= OnAggroChange;
      }
    }
  }
}