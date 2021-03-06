using UnityEngine;

namespace RFG.Items
{
  [CreateAssetMenu(fileName = "New Food Consumable", menuName = "RFG/Items/Consumable/Food Consumable")]
  public class FoodConsumable : Consumable
  {
    public int healthToAdd = 1;

    public override void Consume(Transform transform, Inventory inventory)
    {
      base.Consume(transform, inventory);
      // Debug.Log("Health to add: " + healthToAdd);
    }
  }
}