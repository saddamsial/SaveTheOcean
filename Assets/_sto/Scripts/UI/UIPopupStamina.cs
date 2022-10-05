using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIPopupStamina : MonoBehaviour
{
  [SerializeField] Transform _content;
  [SerializeField] TMPLbl    _lblReward;

  public void Show()
  {
    _lblReward.text = "+" + UIDefaults.GetStaminaString(GameData.Econo.staminaAdReward);
    GetComponent<UIPanel>()?.ActivatePanel();
  }
  public void Hide()
  {
    GetComponent<UIPanel>()?.DeactivatePanel();
  }
  public void OnBtnClick()
  {
    GameState.Econo.stamina += GameData.Econo.staminaAdReward;
    Hide();
  }
}
