using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIEarth : MonoBehaviour
{
  [SerializeField] UIPanel _topPanel;
  [SerializeField] UIPanel _btmPanel;
  [SerializeField] Button  _btnPlay;
  [SerializeField] TMPLbl  _lblLevelInfo;
  [SerializeField] Slider  _slider;


  public static System.Action onBtnPlay;


  float _cleanDst = 0.0f;

  void Awake()
  {
    Earth.onShow += OnEarthShow;
    //Earth.onHide += OnEarthHide;
    Earth.onLevelSelected += UpdateLevelInfo;
    Earth.onLevelStart += OnEarthHide;

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
    GetComponent<UIPanel>().ActivatePanel();
    _topPanel.ActivatePanel();
    _btmPanel.ActivatePanel();
    UpdateLevelInfo(lvlIdx);
    this.Invoke(()=> _cleanDst = GameState.Progress.GetCompletionRate(), 0.25f);
  }
  void Hide()
  {
    _topPanel.DeactivatePanel();
    _btmPanel.DeactivatePanel();
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
    if(_topPanel.IsActive)
      _slider.value = Mathf.Lerp(_slider.value, _cleanDst, Time.deltaTime * 2.0f);
  }
  void Update()
  {
    UpdateSlider();
  }
}
