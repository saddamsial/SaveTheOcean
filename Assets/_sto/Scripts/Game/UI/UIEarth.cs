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

  void Awake()
  {
    Earth.onShow += OnEarthShow;
    Earth.onHide += OnEarthHide;
    Earth.onLevelSelected += UpdateLevelInfo;
  }
  void OnDestroy()
  {
    Earth.onShow -= OnEarthShow;
    Earth.onHide -= OnEarthHide;
    Earth.onLevelSelected -= UpdateLevelInfo;
  }

  private void OnEarthShow(int levelIdx) => Show();
  private void OnEarthHide() => Hide();
  public void Show()
  {
    GetComponent<UIPanel>().ActivatePanel();
    _topPanel.ActivatePanel();
    _btmPanel.ActivatePanel();
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

}
