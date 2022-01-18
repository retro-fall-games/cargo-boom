using UnityEngine;

namespace RFG
{
  [CreateAssetMenu(fileName = "New Money Item", menuName = "RFG/Items/Consumable/Money Item")]
  public class MoneyConsumable : Consumable
  {
    public int amount = 1;

    public override void Consume(Transform transform, Inventory inventory)
    {
      base.Consume(transform, inventory);
      inventory.AddMoney(amount);
    }
  }
}