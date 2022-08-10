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

  float _pollution = 0;
  float _pollutionDest = 0;
  Level _lvl = null;

  void Awake()
  {
    Game.onLevelRestart += OnLevelRestart;
    Level.onCreate += OnLevelStart;
    Level.onFinished += OnLevelFinished;
    Level.onHide += OnLevelHide;
    Level.onTutorialStart += OnTutorialStart;
    Level.onDestroy += OnLevelDestroy;
    Level.onGarbageOut += OnLevelGarbageOut;

    muteBtn.SetState(!GameState.Settings.IsMuted);
  }
  void OnDestroy()
  {
    Game.onLevelRestart -= OnLevelRestart;
    Level.onCreate -= OnLevelStart;
    Level.onFinished -= OnLevelFinished;
    Level.onHide -= OnLevelHide;
    Level.onTutorialStart -= OnTutorialStart;
    Level.onDestroy -= OnLevelDestroy;
    Level.onGarbageOut -= OnLevelGarbageOut;
  }

  public void Show(Level level)
  {
    GetComponent<UIPanel>()?.ActivatePanel();
    _topPanel.ActivatePanel();
  }
  void Hide()
  {
    GetComponent<UIPanel>()?.ActivatePanel();
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
    _lblLevelInfo.text = "Level " + (lvl.levelIdx + 1);

    _progress.minValue = 0;
    _progress.maxValue = 1;
    _progress.value = 1;
    _pollution = 1;
    _pollutionDest = 1;
    
    UpdateScore();
    Show(lvl);
  }
  void OnLevelRestart(Level lvl)
  {
    Hide();
  }
  void OnLevelHide(Level lvl)
  {
    Hide();
  }
  void OnLevelFinished(Level lvl)
  {
    //Hide();
  }
  public void SetLevel(Level lvl)
  {
    _lvl = lvl;
  }
  void UpdateScore()
  {
    _score.text = "";//"Score " + (int)_pointCurr;
  }
  void OnLevelDestroy(Level lvl)
  {
    _lvl = null;
  }
  void OnLevelGarbageOut(Level lvl)
  {
    _pollutionDest = lvl.PollutionRate();
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
    if(_pollution >= _pollutionDest)
    {
      _pollution = Mathf.Lerp(_pollution, _pollutionDest, Time.deltaTime * 2);
      _progress.value = Mathf.Clamp01(1-_pollution);
    }
  }
}
