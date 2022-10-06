using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIPopupStamina : MonoBehaviour
{
  [SerializeField] Transform _content;
  [SerializeField] TMPLbl    _lblReward;

  bool _showAd = false;
  void Awake()
  {
    UnityAdsRewarded.onCompleted += OnRewardedComplete;
  }
  void OnDestroy()
  {
    UnityAdsRewarded.onCompleted -= OnRewardedComplete;
  }
  public void Show()
  {
    _showAd = UnityAdsRewarded.IsReady();
    if(GameState.Events.Popups.noStaminaShown == 0)
    {
      _lblReward.text = "Free " + UIDefaults.GetStaminaString(GameData.Econo.staminaAdReward);
      _showAd = false;
    }
    else
    {
      if(_showAd)
        _lblReward.text = "+" + UIDefaults.GetStaminaString(GameData.Econo.staminaAdReward);
      else
        _lblReward.text = @"no ad :\";  
    }
    
    GetComponent<UIPanel>()?.ActivatePanel();
  }
  public void Hide()
  {
    GetComponent<UIPanel>()?.DeactivatePanel();
  }
  void OnRewardedComplete(string adId)
  {
    GameState.Econo.stamina += GameData.Econo.staminaAdReward;
    GameState.Events.Popups.noStaminaShown++;
  }
  public void OnBtnClick()
  {
    //GameState.Econo.stamina += GameData.Econo.staminaAdReward;
    if(GameState.Events.Popups.noStaminaShown == 0)
      OnRewardedComplete("");
    else
    {  
      if(_showAd)
        UnityAdsRewarded.Show();
    }
    Hide();
  }
}
