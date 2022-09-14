using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIEarth : MonoBehaviour
{
  [SerializeField] Button  _btnPlay;
  [SerializeField] TMPLbl  _lblLevelInfo;
  [SerializeField] Slider  _slider;
  [SerializeField] TMPLbl  _btnStaminaInfo;

  public static System.Action onBtnPlay;

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
    _earthPanel.ActivatePanel();
    UpdateLevelInfo(lvlIdx);
    this.Invoke(()=> _cleanDst = GameState.Progress.GetCompletionRate(), 0.25f);
  }
  void Hide()
  {
    //_globePanel.DeactivatePanel();
    _earthPanel.DeactivatePanel();
  }  

  void UpdateLevelInfo(int level)
  {
    //_btnPlay.interactable = GameState.Progress.Locations.GetLocationState(level) != Level.State.Locked;
    _lblLevelInfo.text = "LEVEL " + (level + 1);
  }

  public void OnBtnPlay()
  {
    if(GameState.Econo.CanSpendStamina(GameData.Econo.staminaCost))
    {
      GameState.Econo.stamina -= GameData.Econo.staminaCost;
      onBtnPlay?.Invoke();
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
