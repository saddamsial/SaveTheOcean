using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;

public class RewardChest : MonoBehaviour
{
  [SerializeField] Slider _slider;
  [SerializeField] GameObject staminaInfo;
  [SerializeField] GameObject coinsInfo;
  [SerializeField] GameObject gemsInfo;
  [SerializeField] TMPLbl lblStamina;
  [SerializeField] TMPLbl lblCoins;
  [SerializeField] TMPLbl lblGems;

  float _rewardPointsMov = 0;

  void Awake()
  {
    GameState.Econo.onRewardProgressChanged += OnRewardChanged;
    _rewardPointsMov = GameState.Econo.rewards;
    OnRewardChanged(_rewardPointsMov);
  }
  void OnDestroy()
  {
    GameState.Econo.onRewardProgressChanged -= OnRewardChanged;
  }

  void SetupSlider()
  {
    UpdateSlider();
  }
  void UpdateInfo()
  {
    lblStamina.text = UIDefaults.staminaIco + string.Format($" x{GameState.Econo.Chest.staminaCnt}");
    lblCoins.text = UIDefaults.coinIco + string.Format($" x{GameState.Econo.Chest.coinsCnt}");
    lblGems.text = UIDefaults.gemIco + string.Format($" x{GameState.Econo.Chest.gemsCnt}");
  }
  void UpdateSlider()
  {
    var rewardProgress = GameData.Econo.GetRewardProgress(_rewardPointsMov);
    _slider.value = rewardProgress.progress_points;
    _slider.minValue = rewardProgress.progress_range_lo;
    _slider.maxValue = rewardProgress.progress_range_hi;
  }
  void OnRewardChanged(float rewardPoints)
  {
    UpdateSlider();
    UpdateInfo();
    
    var rewardProgress = GameData.Econo.GetRewardProgress(rewardPoints);
    if(rewardProgress.lvl > GameState.Econo.Chest.rewardLevel)
    {
      this.Invoke(()=>
      {
        GameState.Econo.Chest.rewardLevel = rewardProgress.lvl;
        GameState.Econo.Chest.AddRewards();
        UpdateInfo();
      }, 0.25f);
    }
  }

  void Update()
  {
    _rewardPointsMov = Mathf.Lerp(_rewardPointsMov, GameState.Econo.rewards, Time.deltaTime * 4);
    UpdateSlider();
  }
}
