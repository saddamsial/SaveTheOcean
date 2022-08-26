using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RewardChest : MonoBehaviour
{
  [SerializeField] Slider _slider;

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
  }

  void Update()
  {
    _rewardPointsMov = Mathf.Lerp(_rewardPointsMov, GameState.Econo.rewards, Time.deltaTime);
    UpdateSlider();
  }
}
