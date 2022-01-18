using UnityEngine;

namespace RFG
{
  public enum FollowState { Following, Pause }

  [AddComponentMenu("RFG/Interactions/Follow")]
  public class Follow : MonoBehaviour
  {
    [field: SerializeField] private FollowState FollowState { get; set; } = FollowState.Following;
    [field: SerializeField] private Transform FollowTransform { get; set; }
    [field: SerializeField] private bool TransformIsPlayer { get; set; } = false;
    [field: SerializeField] private float Speed { get; set; } = 5f;
    [field: SerializeField] private AnimationCurve Tween { get; set; }

    // private float _distancePercent = 0;

    private void Awake()
    {
      if (TransformIsPlayer)
      {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
          FollowTransform = player.transform;
        }
      }
      // Tween.Evaluate(_distancePercent);
    }

    private void LateUpdate()
    {
      if (FollowState == FollowState.Following)
      {
        // float distance = Vector3.Distance(transform.position, FollowTransform.position);
        // float currentDistance = distance;
        // * Tween.Evaluate(_distancePercent)
        transform.position = Vector3.MoveTowards(transform.position, FollowTransform.position, Speed * Time.deltaTime);
        // currentDistance = Vector3.Distance(transform.position, FollowTransform.position);
        // _distancePercent = ((distance - currentDistance)) / distance;
      }
    }

    public void Following()
    {
      FollowState = FollowState.Following;
    }

    public void Pause()
    {
      FollowState = FollowState.Pause;
    }
  }
}