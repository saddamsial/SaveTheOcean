using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RewardChest : MonoBehaviour
{
  [SerializeField] Slider _slider;

  float _sliderDest = 0;

  void Awake()
  {
    GameState.Econo.onRewardProgressChanged += OnRewardChanged;

    _slider.value = 0;
    //var range = GameData.Econo.GetRewardsRange(GameState.Econo.rewards);
    _slider.minValue = 0;//range.beg;
    _slider.maxValue = 10;//range.end;
  }
  void OnDestroy()
  {
    GameState.Econo.onRewardProgressChanged -= OnRewardChanged;
  }

  void OnRewardChanged(int rew_val)
  {
    _sliderDest = rew_val;
  }

  void Update()
  {
    _slider.value = Mathf.Lerp(_slider.value, _sliderDest, Time.deltaTime * 0.5f);
  }
}
