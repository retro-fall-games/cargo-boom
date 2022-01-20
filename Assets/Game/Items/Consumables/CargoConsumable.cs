using UnityEngine;
using RFG.Items;

[CreateAssetMenu(fileName = "New Cargo Consumable", menuName = "RFG/Items/Consumable/Cargo Consumable")]
public class CargoConsumable : Consumable
{
  public int healthToAdd = 1;
  public int armorToAdd = 1;
  public int ammoToAdd = 1;

  public override void Consume(Transform transform, Inventory inventory)
  {
    base.Consume(transform, inventory);
    Debug.Log("Health to add: " + healthToAdd);
  }
}
