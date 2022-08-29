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


  public static System.Action onBtnPlay;

  UIPanel _earthPanel = null;
  float   _cleanDst = 0.0f;

  void Awake()
  {
    Earth.onShow += OnEarthShow;
    //Earth.onHide += OnEarthHide;
    Earth.onLevelSelected += UpdateLevelInfo;
    Earth.onLevelStart += OnEarthHide;

    _earthPanel = GetComponent<UIPanel>();

    _cleanDst = GameState.Progress.GetCompletionRate();
    _slider.minValue = 0;
    _slider.maxValue = 1;
    _slider.value = _cleanDst;
  }
  void OnDestroy()
  {
    Earth.onShow -= OnEarthShow;
    //Earth.onHide -= OnEarthHide;
    Earth.onLevelSelected -= UpdateLevelInfo;
    Earth.onLevelStart -= OnEarthHide;
  }
  private void OnEarthShow(int levelIdx) => Show(levelIdx);
  private void OnEarthHide(int levelIdx) => Hide();
  public void  Show(int lvlIdx)
  {
    _earthPanel.ActivatePanel();
    //_globePanel.ActivatePanel();
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
    _btnPlay.interactable = GameState.Progress.Levels.GetLevelState(level) != Level.State.Locked;
    _lblLevelInfo.text = "LEVEL " + (level + 1);
  }

  public void OnBtnPlay()
  {
    onBtnPlay?.Invoke();
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
