using UnityEngine;

namespace RFG
{
  public class DamageCollider2D : MonoBehaviour
  {
    [field: SerializeField] public int Damage { get; set; } = 0;
  }
}