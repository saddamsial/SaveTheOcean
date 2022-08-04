
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib.DataSystem;

[CreateAssetMenu, DefaultExecutionOrder(-2)]
public class GameState : SavableScriptableObject
{
  static private GameState _static_this = null;
  static public  GameState get() { return _static_this;}
  public GameState()
  {
    _static_this = this;
  }


  [System.Serializable]
  public class Level
  {
    public enum State
    {
      Locked,
      Unlocked,
      Started,
      Finished,
    }

    [SerializeField] int _idx = 0;
    [SerializeField] State _state;
    //other level states data

    public Level(int lvl_idx, State st = State.Unlocked)
    {
      _idx = lvl_idx;
      _state = st;
    }
    public int idx => _idx;
    public State state { get => _state; set { _state = value; } }
  }

  [System.Serializable]
  class ProgressState
  {
    [SerializeField] int _level = 0;
    [SerializeField] List<Level> _levels;


    public  int level { get => _level; set { _level = value; }}
    public  List<Level> levels { get => _levels; }
    public  Level   FindLevel(int lvl_idx)
    {
      return _levels.Find((lvl) => lvl.idx == lvl_idx);
    }
    public  bool    IsLevelUnlocked(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      return (lvl != null)? lvl.state >= Level.State.Unlocked : false;
    }
    public  bool    IsLevelPassed(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      return (lvl != null) ? lvl.state == Level.State.Finished : false;
    }
    public  void    UnlockLevel(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      if(lvl == null)
        _levels.Add(new Level(lvl_idx, Level.State.Unlocked));
      else
      {
        if(lvl.state == Level.State.Locked)
          lvl.state = Level.State.Unlocked;
      }
    }
    public void     PassLevel(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      if(lvl == null)
        _levels.Add(new Level(lvl_idx, Level.State.Finished));
      else
        lvl.state = Level.State.Finished;
    }
  }
  [SerializeField] ProgressState progress;

  [System.Serializable]
  class EconomyState
  {
    public int cash = 0;
  }
  [SerializeField] EconomyState economy;

  [System.Serializable]
  class SettingsState
  {
    [SerializeField] bool _sounds;
    [SerializeField] bool _haptics;

    public bool sounds {get => _sounds; set{_sounds = value;}}
    public bool haptics {get => _haptics; set { _haptics = value; } }
  }
  [SerializeField] SettingsState settings;


  public static void Init()
  {
    get().progress.UnlockLevel(0);

  #if UNITY_EDITOR
    for(int  q = 0; q < GameData.Levels.LevelsCnt; ++q)
    {
      var lvl = GameState.Progress.Levels.GetLevel(q);
      if(lvl != null)
        Debug.Log(string.Format("lvl:{0:D2} => {1}", lvl.idx, lvl.state));
      else
        Debug.Log(string.Format("lvl:{0:D2} => {1}", q, GameState.Level.State.Locked));
    }
  #endif
  }

  public static class Progress
  {
    public static int levelIdx
    {
      get => get().progress.level; 
      set
      {
        get().progress.level = value; 
      }
    }
    public static class Levels
    {
      public static Level GetLevel(int lvl_idx) => get().progress.FindLevel(lvl_idx);
      public static bool  IsLevelUnlocked(int lvl_idx) => get().progress.IsLevelUnlocked(lvl_idx);
      public static bool  IsLevelFinished(int lvl_idx) => get().progress.IsLevelPassed(lvl_idx);
      public static void  SetLevelFinished(int lvl_idx) => get().progress.PassLevel(lvl_idx);
      public static void  SetLevelFinished() => SetLevelFinished(levelIdx);
      public static void  UnlockNextLevel(int lvl_idx) => get().progress.UnlockLevel(GameData.Levels.NextLevel(lvl_idx));
      public static void  UnlockNextLevel() => UnlockNextLevel(levelIdx);
    }
  }
  public static class Econo
  {
    public static int Cash {get => get().economy.cash; set{ get().economy.cash = value;}}
  }
  public static class Settings
  {
    public static bool IsMuted {get => !get().settings.sounds; set{get().settings.sounds = !value;}}
  }

  [Header("Customization")]
  [SerializeField] int selectedTheme = 0;
  public int SetNextTheme() {
    selectedTheme = (int)Mathf.Repeat(++selectedTheme, GameData.GetThemeColors().Length);
    return selectedTheme;
  }
}
