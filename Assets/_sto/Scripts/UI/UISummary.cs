using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameLib.UI;
using TMPLbl = TMPro.TextMeshProUGUI;

public class UISummary : MonoBehaviour
{
  public static System.Action onShow, onBtnPlay;

  //[SerializeField] UIPanel _winContainer;
  //[SerializeField] TMPLbl  _levelInfo;

  public void Show(Level level)
  {
    //_levelInfo.text = "Level " + (level.levelIdx + 1);
    onShow?.Invoke();

    //_winContainer.gameObject.SetActive(level.succeed);
    GetComponent<UIPanel>().ActivatePanel();
  }
  void Hide()
  {
    GetComponent<UIPanel>().DeactivatePanel();
  }
  public void OnBtnRestart()
  {
    Hide();
    FindObjectOfType<Game>().RestartLevel();
  }
  public void OnBtnPlay()
  {
    Hide();
    onBtnPlay?.Invoke();
    FindObjectOfType<Game>().ShowEarth(true);
  }
}
