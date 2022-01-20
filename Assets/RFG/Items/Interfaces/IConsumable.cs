using UnityEngine;

namespace RFG.Items
{
  public interface IConsumable
  {
    void Consume(Transform transform, Inventory inventory);
  }
}