using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib;

public class RewardChest : MonoBehaviour
{
  [SerializeField] Slider _slider;
  [SerializeField] TMPLbl _lblResCnt;
  [SerializeField] GameObject _infoContainer;
  [SerializeField] Transform _chestLid;

  public static System.Action<RewardChest> onPoped, onNotPoped;

  float _rewardPointsMov = 0;
  float _lidAngle = 0;
  public static int layerMask = 0;

  void Awake()
  {
    GameState.Econo.onRewardProgressChanged += OnRewardChanged;
    _rewardPointsMov = GameState.Econo.rewards;
    OnRewardChanged(_rewardPointsMov);

    layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

    _lidAngle = (_resCnt == 0) ? 0 : 90;

    this.Invoke(()=> GetComponent<ActivatableObject>().ActivateObject(), 1.0f);
  }
  void OnDestroy()
  {
    GameState.Econo.onRewardProgressChanged -= OnRewardChanged;
  }

  int _resCnt => GameState.Chest.staminaCnt + GameState.Chest.coinsCnt + GameState.Chest.gemsCnt;

  void SetupSlider()
  {
    UpdateSlider();
  }
  void UpdateInfo()
  {
    _lblResCnt.text = $"x{_resCnt}";
    _infoContainer.gameObject.SetActive(_resCnt > 0);
  }
  void UpdateSlider()
  {
    var rewardProgress = GameData.Econo.GetRewardProgress(_rewardPointsMov);
    _slider.value = rewardProgress.progress_points;
    _slider.minValue = rewardProgress.progress_range_lo;
    _slider.maxValue = rewardProgress.progress_range_hi;
  }
  void UpdateLid()
  {
    float angleTo = (_resCnt == 0) ? 0 : 90;
    _lidAngle = Mathf.Lerp(_lidAngle, angleTo, Time.deltaTime * 5);
    _chestLid.localRotation = Quaternion.AngleAxis(_lidAngle, Vector3.right);
  }  
  void OnRewardChanged(float rewardPoints)
  {
    UpdateSlider();
    UpdateInfo();
    
    var rewardProgress = GameData.Econo.GetRewardProgress(rewardPoints);
    if(rewardProgress.lvl > GameState.Chest.rewardLevel)
    {
      this.Invoke(()=>
      {
        GameState.Chest.rewardLevel = rewardProgress.lvl;
        GameState.Chest.AddRewards();
        UpdateInfo();
      }, 0.25f);
    }
  }

  public Item.ID? Pop()
  {
    var id = GameState.Chest.PopRes();
    UpdateInfo();
    if(id != null)
      onPoped?.Invoke(this);
    else
      onNotPoped?.Invoke(this);  
    return id;  
  }
  void Update()
  {
    _rewardPointsMov = Mathf.Lerp(_rewardPointsMov, GameState.Econo.rewards, Time.deltaTime * 4);
    UpdateSlider();
    UpdateLid();
  }
}
