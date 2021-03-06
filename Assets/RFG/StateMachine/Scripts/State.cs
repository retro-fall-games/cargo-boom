using System;
using UnityEngine;

namespace RFG
{
  public class State : ScriptableObject
  {
    [Header("Animations")]
    [Tooltip("Define what layer to play animations")]
    public string Layer = "Base Layer";

    [Tooltip("Define what animation to run when the state enters")]
    public string EnterClip;

    [Tooltip("Define what animation to run when the state exits")]
    public string ExitClip;

    [Header("Effects")]
    [Tooltip("Define what effects to run when the state exits")]
    public string[] EnterEffects;
    public bool EnterEffectsParent = false;
    public bool EnterEffectsGrandParent = false;
    public bool EnterEffectsWorldPositionStays = false;

    [Tooltip("Define what effect to run when the state exits")]
    public string[] ExitEffects;
    public bool ExitEffectsParent = false;
    public bool ExitEffectsGrandParent = false;
    public bool ExitEffectsWorldPositionStays = false;

    public bool StopEnterEffectsOnExit = false;

    [Header("State Conditions")]
    public bool FreezeState = false;
    public float WaitToUnfreezeTime = 0f;
    public State[] StatesCanUnfreeze;

    [Header("State Complete")]
    public State NextState;
    public float NextStateAfterTime = 0f;
    public bool GoToNextStateAfterCompletion = false;

    public virtual void Enter(IStateContext context)
    {
      PlayEffects(context, EnterEffects, EnterEffectsParent, EnterEffectsGrandParent, EnterEffectsWorldPositionStays);
      PlayAnimations(context, EnterClip);
    }

    public virtual Type Execute(IStateContext context)
    {
      return null;
    }

    public virtual void Exit(IStateContext context)
    {
      if (StopEnterEffectsOnExit)
      {
        StopEffects(context, EnterEffects);
      }
      PlayEffects(context, ExitEffects, ExitEffectsParent, ExitEffectsGrandParent, ExitEffectsWorldPositionStays);
      PlayAnimations(context, ExitClip);
    }

    protected void PlayAnimations(IStateContext context, string clip)
    {
      StateAnimatorContext animatorContext = context as StateAnimatorContext;
      if (animatorContext.animator == null)
      {
        return;
      }
      animatorContext.animator.PlayClip(clip, Layer);
    }

    protected void PlayEffects(IStateContext context, string[] effects, bool parent, bool grandParent, bool worldPositionStays)
    {
      StateTransformContext transformContext = context as StateTransformContext;
      if (grandParent)
      {
        transformContext.transform.SpawnFromPool(effects, transformContext.transform.parent, worldPositionStays);
      }
      else if (parent)
      {
        transformContext.transform.SpawnFromPool(effects, transformContext.transform, worldPositionStays);
      }
      else
      {
        transformContext.transform.SpawnFromPool(effects);
      }
    }

    protected void StopEffects(IStateContext context, string[] effects)
    {
      StateTransformContext transformContext = context as StateTransformContext;
      transformContext.transform.DeactivatePoolByTag(effects);
    }
  }
}