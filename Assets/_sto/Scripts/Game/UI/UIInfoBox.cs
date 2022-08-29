using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;

public class UIInfoBox : MonoBehaviour
{
  [SerializeField] TMPLbl _info;
  [SerializeField] Slider _slider;
  [SerializeField] Item.Kind _resKind;

  public int resValue 
  {
    set => _info.text = _resKind switch
    {
      Item.Kind.Stamina => UIDefaults.GetStaminaString(value),
      Item.Kind.Gem => UIDefaults.GetGemsString(value),
      Item.Kind.Coin => UIDefaults.GetCoinsString(value),
      _ => value.ToString()
    };
  }
}
