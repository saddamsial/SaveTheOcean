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
  [SerializeField] TMPLbl _lblLevelInfo;
  [SerializeField] Slider _slider;

  float _cleanDst = 0.0f;

  void Awake()
  {
    Earth.onShow += OnEarthShow;
    Earth.onHide += OnEarthHide;
    Earth.onLevelSelected += UpdateLevelInfo;

    _cleanDst = GameState.Progress.GetCompletionRate();
    _slider.minValue = 0;
    _slider.maxValue = 1;
    _slider.value = _cleanDst;
  }
  void OnDestroy()
  {
    Earth.onShow -= OnEarthShow;
    Earth.onHide -= OnEarthHide;
    Earth.onLevelSelected -= UpdateLevelInfo;
  }

  private void OnEarthShow(int levelIdx) => Show();
  private void OnEarthHide() => Hide();
  public void  Show()
  {
    GetComponent<UIPanel>().ActivatePanel();
    _topPanel.ActivatePanel();
    _btmPanel.ActivatePanel();
    this.Invoke(()=> _cleanDst = GameState.Progress.GetCompletionRate(), 0.25f);
  }
  void Hide()
  {
    _topPanel.DeactivatePanel();
    _btmPanel.DeactivatePanel();
  }  

  void UpdateLevelInfo(int level)
  {
    _lblLevelInfo.text = "LEVEL: " + (level + 1);
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
