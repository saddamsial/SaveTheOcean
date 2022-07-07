using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIIngame : MonoBehaviour
{
  [Header("TopPanelRefs")]
  [SerializeField] Slider     _progress;
  [SerializeField] TMPLbl     _score;
  [SerializeField] UIPanel    _topPanel;
  [SerializeField] TMPLbl     _lblLevelInfo;
  
  [Header("Settings")]
  [SerializeField] UIToggleButton muteBtn;

  int   _pointDest = 0;
  float _pointCurr = 0;
  Level _lvl = null;

  void Awake()
  {
    Game.onLevelRestart += OnLevelRestart;
    Level.onCreate += OnLevelStart;
    Level.onFinished += OnLevelFinished;
    Level.onTutorialStart += OnTutorialStart;
    Level.onDestroy += OnLevelDestroy;

    muteBtn.SetState(!GameState.Settings.IsMuted);
    //ApplySettings();
  }
  void OnDestroy()
  {
    Game.onLevelRestart -= OnLevelRestart;
    Level.onCreate -= OnLevelStart;
    Level.onFinished -= OnLevelFinished;
    Level.onTutorialStart -= OnTutorialStart;
    Level.onDestroy -= OnLevelDestroy;
  }

  public void Show(Level level)
  {
    GetComponent<UIPanel>()?.ActivatePanel();
    _topPanel.ActivatePanel();
  }
  void Hide()
  {
    _topPanel.DeactivatePanel();
  }

  void OnLevelCreated(Level lvl)
  {
    _lvl = lvl;
    OnLevelStart(lvl);
  }
  void OnLevelStart(Level lvl)
  {
    _lvl = lvl;
    //_lblLevelInfo.text = "Level " + (lvl.LevelIdx + 1);

    // _progress.minValue = 0;
    // _progress.value = 0;
    // _progress.maxValue = lvl.PointsMax;
    _pointCurr = 0;
    _pointDest = 0;
    
    UpdateScore();
    Show(lvl);
  }
  void OnLevelRestart(Level lvl)
  {
    Hide();
  }
  void OnLevelFinished(Level lvl)
  {
    Hide();
  }
  public void SetLevel(Level lvl)
  {
    _lvl = lvl;
  }
  void UpdateScore()
  {
    //_score.text = "Score " + (int)_pointCurr;
  }
  void OnLevelDestroy(Level lvl)
  {
    _lvl = null;
  }
  void OnPointsAdded(Level lvl)
  {
    _pointDest = lvl.Points;
  }
  void OnTutorialStart(Level lvl)
  {

  }
  public void OnBtnRestart()
  {
    FindObjectOfType<Game>()?.RestartLevel();
  }
  public void OnBtnMute()
  {
    GameState.Settings.IsMuted = !GameState.Settings.IsMuted;
  }

  void Update()
  {
    if(_pointCurr != _pointDest)
    {
      _pointCurr = Mathf.MoveTowards(_pointCurr, _pointDest, Time.deltaTime * 1200);
      _progress.value = _pointCurr;
      UpdateScore();
    }
  }
}
