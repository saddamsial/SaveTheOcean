
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
  class StateProgress
  {
    public int level = 0;
  }
  [SerializeField] StateProgress progress;

  [System.Serializable]
  class Economy
  {
    public int cash = 0;
  }
  [SerializeField] Economy economy;

  [System.Serializable]
  class GameSettings
  {
    [SerializeField] bool _sounds;
    [SerializeField] bool _haptics;

    public bool sounds {get => _sounds; set{_sounds = value;}}
    public bool haptics {get => _haptics; set { _haptics = value; } }
  }
  [SerializeField] GameSettings settings;

  public static class Progress
  {
    public static int   Level 
    {
      get => get().progress.level; 
      set
      {
        get().progress.level = value; 
      }
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
