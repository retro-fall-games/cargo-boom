using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Interactions/Trigger Deactivate Game Object"), RequireComponent(typeof(BoxCollider2D))]
  public class TriggerDeactivateGameObject : MonoBehaviour
  {
    public string[] Tags;

    public void OnTriggerEnter2D(Collider2D col)
    {
      if (col.gameObject.CompareTags(Tags))
      {
        col.gameObject.SetActive(false);
      }
    }

  }
}