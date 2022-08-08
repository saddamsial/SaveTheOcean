using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIEarth : MonoBehaviour
{
  [SerializeField] UIPanel _topPanel;
  [SerializeField] TMPLbl _lblLevelInfo;

  public void Show()
  {
    GetComponent<UIPanel>()?.ActivatePanel();
    _topPanel.ActivatePanel();
  }
  void Hide()
  {
    _topPanel.DeactivatePanel();
  }  

}
