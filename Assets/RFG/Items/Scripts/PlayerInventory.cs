using UnityEngine;

namespace RFG.Items
{
  [AddComponentMenu("RFG/Items/Player Inventory")]
  [DefaultExecutionOrder(-100)]
  public class PlayerInventory : MonoBehaviour
  {
    [field: SerializeField] public Inventory Inventory { get; set; }
    [field: SerializeField] private bool Clone { get; set; } = false;

    private void Awake()
    {
      if (Clone)
      {
        Inventory = Inventory.Clone();
      }
    }
  }
}