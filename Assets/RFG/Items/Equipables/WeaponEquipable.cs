using UnityEngine;

namespace RFG.Items
{
  public enum WeaponEquipableType { InstaFire, Chargable, MachineGun, HitScan }

  [CreateAssetMenu(fileName = "New Weapon Equipable", menuName = "RFG/Items/Equipables/Weapon Equipable")]
  public class WeaponEquipable : Equipable
  {
    public WeaponEquipableType WeaponEquipableType;

    public virtual void Started()
    {
    }

    public virtual void Cancel()
    {
    }

    public virtual void Perform()
    {
    }
  }
}