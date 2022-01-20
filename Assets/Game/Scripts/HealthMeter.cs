using UnityEngine;
using UnityEngine.UI;
using RFG;
using RFG.Character;
using RFG.ScrollingShooter;

public class HealthMeter : MonoBehaviour
{
  [field: SerializeField] private Slider HealthSlider { get; set; }
  [field: SerializeField] private Slider ArmorSlider { get; set; }
  [field: SerializeField] private RFG.ScrollingShooter.Character Character { get; set; }
  private HealthBehaviour _healthBehaviour;

  private void Awake()
  {
    _healthBehaviour = Character.gameObject.GetComponent<HealthBehaviour>();
  }

  private void OnEnable()
  {
    _healthBehaviour.OnHealthChange += OnHealthChange;
    _healthBehaviour.OnArmorChange += OnArmorChange;
  }

  private void OnDisable()
  {
    _healthBehaviour.OnHealthChange += OnHealthChange;
    _healthBehaviour.OnArmorChange += OnArmorChange;
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
}
