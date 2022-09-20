using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib;
using GameLib.Utilities;

public class RewardChest : MonoBehaviour
{
  [SerializeField] GameObject _content;
  [SerializeField] Slider _slider;
  [SerializeField] TMPLbl _lblResCnt;
  [SerializeField] GameObject _infoContainer;
  [SerializeField] Transform _chestLid;
  [SerializeField] ObjectShake _shake;

  public static System.Action<RewardChest> onPoped, onNotPoped, onNotPushed, onReward, onShow;

  float _rewardPointsMov = 0;
  float _lidAngle = 0;
  public static int layerMask = 0;

  public int level => GameState.Chest.rewardLevel;

  int _resCnt => GameState.Chest.staminaCnt + GameState.Chest.coinsCnt + GameState.Chest.gemsCnt;

  void Awake()
  {
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

    GameState.Econo.onRewardProgressChanged += OnRewardChanged;

    _rewardPointsMov = GameState.Econo.rewards;
    OnRewardChanged(_rewardPointsMov);

    _content.SetActive(false);
    if(GameState.Chest.ShouldShow())
      Show();
  }
  void OnDestroy()
  {
    GameState.Econo.onRewardProgressChanged -= OnRewardChanged;
  }

  public void Show()
  {
    _lidAngle = (_resCnt == 0) ? 0 : 90;
    _content.SetActive(true);
    GameState.Chest.shown = true;
    this.Invoke(() => { GetComponent<ActivatableObject>().ActivateObject(); onShow?.Invoke(this); }, 1.0f);
  }

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
      if(rewardProgress.lvl == 1)
        Show();
      onReward?.Invoke(this);
      this.Invoke(()=>
      {
        GameState.Chest.rewardLevel = rewardProgress.lvl;
        GameState.Chest.AddRewards();
        UpdateInfo();
      }, 0.25f);
    }
  }
  public void NoPush(Item.ID id)
  {
    _shake.Shake();
    onNotPushed?.Invoke(this);
  }
  public Item.ID? Pop()
  {
    var id = GameState.Chest.PopRes();
    _shake.Shake();    
    UpdateInfo();
    if(id != null)
      onPoped?.Invoke(this);
    else
      onNotPoped?.Invoke(this);
    return id;  
  }
  void Update()
  {
    if(_content.activeSelf)  
    {
      _rewardPointsMov = Mathf.Lerp(_rewardPointsMov, GameState.Econo.rewards, Time.deltaTime * 4);
      UpdateSlider();
      UpdateLid();
    }
  }
}
