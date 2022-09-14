using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.UI;

public class UIPopupStamina : MonoBehaviour
{
  [SerializeField] Transform _content;

  public void Show()
  {
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
