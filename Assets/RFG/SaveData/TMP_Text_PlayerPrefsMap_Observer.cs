using UnityEngine;
using TMPro;

namespace RFG
{
  [AddComponentMenu("RFG/Save Data/TMP Text Player Prefs Map Observer")]
  public class TMP_Text_PlayerPrefsMap_Observer : MonoBehaviour
  {
    [field: SerializeField] private PlayerPrefsMap PlayerPrefsMap { get; set; }
    [field: SerializeField] private string Name { get; set; }
    private TMP_Text _text;

    private void Awake()
    {
      _text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
      _text.SetText(PlayerPrefsMap.GetItem(Name).StringValue);
    }

    private void OnEnable()
    {
      PlayerPrefsMap.OnUpdate += OnUpdate;
    }

    private void OnDisable()
    {
      PlayerPrefsMap.OnUpdate += OnUpdate;
    }

    private void OnUpdate()
    {
      _text.SetText(PlayerPrefsMap.GetItem(Name).StringValue);
    }
  }
}