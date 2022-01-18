using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Platformer/Character/Behaviours/Health")]
  public class HealthBehaviour : MonoBehaviour
  {
    [Header("Health")]
    public FloatReference HealthReference;
    public FloatReference MaxHealthReference;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float DefaultMaxHealth = 100f;

    [Header("Armor")]
    public FloatReference ArmorReference;
    public FloatReference MaxArmorReference;
    public float Armor = 100f;
    public float MaxArmor = 100f;
    public float DefaultMaxArmor = 100f;

    [Header("Game Events")]
    public GameEvent KillEvent;

    private Character _character;

    private void Awake()
    {
      _character = GetComponent<Character>();
      if (HealthReference != null)
      {
        Health = HealthReference.Value;
      }
      if (MaxHealthReference != null)
      {
        MaxHealthReference.Value = DefaultMaxHealth;
        MaxHealth = MaxHealthReference.Value;
      }
      if (ArmorReference != null)
      {
        Armor = ArmorReference.Value;
      }
      if (MaxArmorReference != null)
      {
        MaxArmorReference.Value = DefaultMaxArmor;
        MaxArmor = MaxArmorReference.Value;
      }
    }

    public void TakeDamage(float damage)
    {
      SetArmor(Armor - damage);
      if (Armor <= 0)
      {
        SetHealth(Health - damage);
      }
    }

    #region Health
    public void SetHealth(float amount)
    {
      if (amount >= MaxHealth)
      {
        amount = MaxHealth;
      }
      Health = amount;
      if (Health <= 0)
      {
        Health = 0;
      }
      if (HealthReference != null)
      {
        HealthReference.Value = Health;
      }
      if (Health <= 0)
      {
        KillEvent?.Raise();
        _character.Kill();
      }
    }

    public void AddHealth(float amount)
    {
      SetHealth(Health += amount);
    }

    public void ResetHealth()
    {
      SetHealth(MaxHealth);
    }

    public void AddMaxHealth(float amount)
    {
      MaxHealth += amount;
      if (MaxHealth <= 1)
      {
        MaxHealth = 1;
      }
      if (MaxHealthReference != null)
      {
        MaxHealthReference.Value = MaxHealth;
      }
    }
    #endregion

    #region Armor
    public void SetArmor(float amount)
    {
      if (amount >= MaxArmor)
      {
        amount = MaxArmor;
      }
      Armor = amount;
      if (Armor <= 0)
      {
        Armor = 0;
      }
      if (ArmorReference != null)
      {
        ArmorReference.Value = Armor;
      }
    }

    public void AddArmor(float amount)
    {
      SetArmor(Armor += amount);
    }

    public void ResetArmor()
    {
      SetArmor(MaxArmor);
    }

    public void AddMaxArmor(float amount)
    {
      MaxArmor += amount;
      if (MaxArmor <= 1)
      {
        MaxArmor = 1;
      }
      if (MaxArmorReference != null)
      {
        MaxArmorReference.Value = MaxArmor;
      }
    }
    #endregion

  }
}
