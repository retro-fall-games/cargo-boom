using UnityEngine;
using TMPro;

namespace RFG
{
  [AddComponentMenu("RFG/Items/TMP Text Money Observer")]
  public class TMP_Text_MoneyObserver : MonoBehaviour
  {
    [field: SerializeField] public Inventory Inventory { get; set; }
    private TMP_Text _money;

    private void Awake()
    {
      _money = GetComponent<TMP_Text>();
    }

    private void Start()
    {
      _money.SetText(Inventory.Money.ToString());
    }

    private void OnEnable()
    {
      Inventory.OnUpdateMoney += OnUpdateMoney;
    }

    private void OnDisable()
    {
      Inventory.OnUpdateMoney += OnUpdateMoney;
    }

    private void OnUpdateMoney(int money)
    {
      _money.SetText(money.ToString());
    }
  }
}