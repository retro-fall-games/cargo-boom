using UnityEngine;

namespace RFG.Items
{
  public interface IEquipable
  {
    void Equip(Transform transform, Inventory inventory);
    void Unequip(Transform transform, Inventory inventory);
  }
}