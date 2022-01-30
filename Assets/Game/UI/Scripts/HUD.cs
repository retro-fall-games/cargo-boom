using UnityEngine;
using UnityEngine.UI;
using RFG;
using RFG.Items;
using RFG.Character;
using RFG.ScrollingShooter;
using RFG.Weapons;
using TMPro;

public class HUD : MonoBehaviour
{
  [field: SerializeField] private Slider HealthSlider { get; set; }
  [field: SerializeField] private Slider ArmorSlider { get; set; }
  [field: SerializeField] private TMP_Text MissleCount { get; set; }
  [field: SerializeField] private RFG.ScrollingShooter.Character Character { get; set; }
  private HealthBehaviour _healthBehaviour;
  private PlayerInventory _playerInventory;
  private ProjectileWeaponEquipable _secondaryWeaponEquipable;

  private void Awake()
  {
    _healthBehaviour = Character.gameObject.GetComponent<HealthBehaviour>();
    _playerInventory = Character.gameObject.GetComponent<PlayerInventory>();

    if (_playerInventory != null && _playerInventory.Inventory != null)
    {
      _secondaryWeaponEquipable = _playerInventory.Inventory.RightHand as ProjectileWeaponEquipable;
    }
  }

  private void OnEnable()
  {
    _healthBehaviour.OnHealthChange += OnHealthChange;
    _healthBehaviour.OnArmorChange += OnArmorChange;
    _secondaryWeaponEquipable.OnAmmoChange += OnAmmoChange;
  }

  private void OnDisable()
  {
    _healthBehaviour.OnHealthChange -= OnHealthChange;
    _healthBehaviour.OnArmorChange -= OnArmorChange;
    _secondaryWeaponEquipable.OnAmmoChange -= OnAmmoChange;
  }

  private void OnHealthChange(FloatReference maxHealth, FloatReference health)
  {
    float percentage = health.Value / maxHealth.Value;
    HealthSlider.value = percentage;
  }

  private void OnArmorChange(FloatReference maxArmor, FloatReference armor)
  {
    float percentage = armor.Value / maxArmor.Value;
    ArmorSlider.value = percentage;
  }

  private void OnAmmoChange(int amount)
  {
    MissleCount.SetText(amount.ToString());
  }
}
