using UnityEngine;

namespace RFG
{
  public static class AnimatorEx
  {
    public static void ResetCurrentClip(this Animator animator)
    {
      AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
      int stateName = currentState.fullPathHash;
      animator.Play(stateName, 0, 0.0f);
      animator.speed = 1;
    }

    public static bool HasClip(this Animator animator, string clip, string layer = "Base Layer")
    {
      int hash = Animator.StringToHash(clip);
      int layerIndex = animator.GetLayerIndex(layer);
      return animator.HasState(layerIndex, hash);
    }

    public static void PlayClip(this Animator animator, string clip, string layer = "Base Layer")
    {
      if (!string.IsNullOrEmpty(clip) && animator.HasClip(clip, layer))
      {
        animator.Play(clip);
      }
    }

  }
}