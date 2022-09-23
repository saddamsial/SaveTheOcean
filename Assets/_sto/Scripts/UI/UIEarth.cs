using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIEarth : MonoBehaviour
{
  [SerializeField] Button  _btnPlay;
  [SerializeField] Button  _btnFeed;
  [SerializeField] TMPLbl  _lblLevelInfo;
  [SerializeField] Slider  _slider;
  [SerializeField] TMPLbl  _btnActionInfo;
  [SerializeField] TMPLbl  _btnStaminaInfo;

  public static System.Action onBtnPlay, onBtnFeed;

  UIPanel _earthPanel = null;
  float   _cleanDst = 0.0f;

  void Awake()
  {
    Earth.onShow += OnEarthShow;
    Earth.onLevelSelected += UpdateLevelInfo;
    Earth.onLevelStart += OnEarthHide;

    _earthPanel = GetComponent<UIPanel>();

    _cleanDst = GameState.Progress.GetCompletionRate();
    _slider.minValue = 0;
    _slider.maxValue = 1;
    _slider.value = _cleanDst;

    _btnStaminaInfo.text = UIDefaults.GetStaminaString(GameData.Econo.staminaCost);
  }
  void OnDestroy()
  {
    Earth.onShow -= OnEarthShow;
    Earth.onLevelSelected -= UpdateLevelInfo;
    Earth.onLevelStart -= OnEarthHide;
  }
  private void OnEarthShow(int levelIdx) => Show(levelIdx);
  private void OnEarthHide(int levelIdx) => Hide();
  public void  Show(int lvlIdx)
  {
    //_btnFeed.gameObject.SetActive(GameState.Progress.Locations.GetFinishedCnt() >= GameData.Levels.GetFeedingAvailLoc());
    
    _earthPanel.ActivatePanel();
    UpdateLevelInfo(lvlIdx);
    this.Invoke(()=> _cleanDst = GameState.Progress.GetCompletionRate(), 0.25f);
  }
  void Hide()
  {
    _earthPanel.DeactivatePanel();
  }  

  bool IsLocationSelectable(int location)
  {
    var state = GameState.Progress.Locations.GetLocationState(location);
    return state != Level.State.Locked;// && state != Level.State.Finished;
  }
  void UpdateLevelInfo(int location)
  {
    _btnPlay.interactable = IsLocationSelectable(location);
    _lblLevelInfo.text = "LEVEL " + (location + 1);
  }

  public void OnBtnPlay()
  {
    if(GameState.Progress.Locations.GetLocationState(GameState.Progress.locationIdx) != Level.State.Finished)
    {
      if(GameState.Econo.CanSpendStamina(GameData.Econo.staminaCost))
      {
        GameState.Econo.stamina -= GameData.Econo.staminaCost;
        onBtnPlay?.Invoke();
      }
      else
        FindObjectOfType<UIPopupStamina>(true)?.Show();
    }
    else
      OnBtnFeed();
  }
  public void OnBtnFeed()
  {
    if(GameState.Econo.CanSpendStamina(GameData.Econo.staminaFeedCost))
    {
      GameState.Econo.stamina -= GameData.Econo.staminaFeedCost;
      onBtnFeed?.Invoke();
    }
    else
      FindObjectOfType<UIPopupStamina>(true)?.Show();
  }

  void UpdateSlider()
  {
    if(_earthPanel?.IsActive ?? false)
       _slider.value = Mathf.Lerp(_slider.value, _cleanDst, Time.deltaTime * 2.0f);
  }
  void Update()
  {
    UpdateSlider();
  }
}
