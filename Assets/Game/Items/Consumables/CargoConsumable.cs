using UnityEngine;
using RFG.Character;
using RFG.Items;
using RFG.Weapons;

public enum CargoType { Health, Armor, Ammo, PowerUp };

[CreateAssetMenu(fileName = "New Cargo Consumable", menuName = "RFG/Items/Consumable/Cargo Consumable")]
public class CargoConsumable : Consumable
{
  public CargoType CargoType;
  public int Amount;

  public override void Consume(Transform transform, Inventory inventory)
  {
    base.Consume(transform, inventory);
    switch (CargoType)
    {
      case CargoType.Health:
        HealthBehaviour health = transform.gameObject.GetComponent<HealthBehaviour>();
        if (health != null)
        {
          health.AddHealth(Amount);
        }
        break;
      case CargoType.Armor:
        HealthBehaviour armor = transform.gameObject.GetComponent<HealthBehaviour>();
        if (armor != null)
        {
          armor.AddArmor(Amount);
        }
        break;
      case CargoType.Ammo:
        ProjectileWeaponEquipable weapon = inventory.RightHand as ProjectileWeaponEquipable;
        if (weapon != null)
        {
          weapon.AddAmmo(Amount);
        }
        break;
      case CargoType.PowerUp:
        Debug.Log("Power Up");
        break;
    }
  }
}
