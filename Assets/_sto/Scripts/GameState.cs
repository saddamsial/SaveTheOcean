using System;
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
  public static void Process()
  {
    //get()?.economy?.Process();
    GameState.Econo.Process();
  }

  [System.Serializable]
  class LevelState
  {
    [SerializeField] int _idx = 0;
    [SerializeField] Level.State _state;
    //other level states data

    public LevelState(int lvl_idx, Level.State st = Level.State.Unlocked)
    {
      _idx = lvl_idx;
      _state = st;
    }
    public int idx => _idx;
    public Level.State state { get => _state; set { _state = value; } }
  }

  [System.Serializable]
  class ProgressState
  {
    [SerializeField] int _level = 0;
    [SerializeField] List<LevelState> _levels;


    public  int level { get => _level; set { _level = value; }}
    public  List<LevelState> levels { get => _levels; }
    public  LevelState       FindLevel(int lvl_idx)
    {
      return _levels.Find((lvl) => lvl.idx == lvl_idx);
    }
    public  bool        IsLevelUnlocked(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      return (lvl != null)? lvl.state >= Level.State.Unlocked : false;
    }
    public  bool        IsLevelPassed(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      return (lvl != null) ? lvl.state == Level.State.Finished : false;
    }
    public  void        UnlockLevel(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      if(lvl == null)
        _levels.Add(new LevelState(lvl_idx, Level.State.Unlocked));
      else
      {
        if(lvl.state == Level.State.Locked)
          lvl.state = Level.State.Unlocked;
      }
    }
    public void         PassLevel(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      if(lvl == null)
        _levels.Add(new LevelState(lvl_idx, Level.State.Finished));
      else
        lvl.state = Level.State.Finished;
    }
    public Level.State  GetLevelState(int lvl_idx)
    {
      var lvl = FindLevel(lvl_idx);
      return (lvl != null) ? lvl.state : Level.State.Locked;
    }
  }
  [SerializeField] ProgressState progress;

  [System.Serializable]
  class EconomyState
  {
    public int    stamina = 50;
    public int    coins = 0;
    public int    gems = 0;
    public float  rewardPoints = 0;
    public int    rewardLevel = 0;
    public long   lastStamina = 0;
    [System.Serializable]
    public class ChestState
    {
      public List<Item.ID> listStamina = new List<Item.ID>();
      public List<Item.ID> listCoins = new List<Item.ID>();
      public List<Item.ID> listGems = new List<Item.ID>();

      public void AddReward(GameData.Rewards.Reward rew)
      {
        listStamina.Add(new Item.ID(0 , 0, Item.Kind.Stamina));
        listCoins.Add(new Item.ID(0, 0, Item.Kind.Coin));
        listGems.Add(new Item.ID(0, 0, Item.Kind.Gem));
      }
    }
    public ChestState chestState = new ChestState();
  }
  [SerializeField] EconomyState economy;

  [System.Serializable]
  class SplitMachineState
  {
    [SerializeField] int _capacity = 1;

    public int capacity {get => _capacity ; set => _capacity = value;}
  }
  [SerializeField] SplitMachineState splitMachine;
  
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
    if(get().economy.lastStamina == 0)
      get().economy.lastStamina = CTime.get().ToBinary();

  #if UNITY_EDITOR
    for(int  q = 0; q < GameData.Levels.LevelsCnt; ++q)
    {
      Debug.Log(string.Format("lvl:{0:D2} => {1}", q, GameState.Progress.Levels.GetLevelState(q)));
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
      public static Level.State   GetLevelState(int lvl_idx) => get().progress.GetLevelState(lvl_idx);
      //public static void        SetLevelState(int lvl_idx, Level.State state) => get().progress.SetLevelState(lvl_idx, state);
      public static bool          IsLevelUnlocked(int lvl_idx) => get().progress.IsLevelUnlocked(lvl_idx);
      public static bool          IsLevelFinished(int lvl_idx) => get().progress.IsLevelPassed(lvl_idx);
      public static void          SetLevelFinished(int lvl_idx) => get().progress.PassLevel(lvl_idx);
      public static void          SetLevelFinished() => SetLevelFinished(levelIdx);
      public static void          UnlockNextLevel(int lvl_idx) => get().progress.UnlockLevel(GameData.Levels.NextLevel(lvl_idx));
      public static void          UnlockNextLevel() => UnlockNextLevel(levelIdx);
      public static Level.State[] GetStates()
      {
        var states = new Level.State[GameData.Levels.LevelsCnt];
        for(int q = 0; q < states.Length; q++)
          states[q] = GetLevelState(q);

        return states;  
      }
    }

    public static float GetCompletionRate()
    {
      Level.State[] states = Levels.GetStates();
      float finishedCnt = states.Where((state) => state == Level.State.Finished).Count();
      return finishedCnt / GameData.Levels.LevelsCnt;
    }
  }
  public static class Econo
  {
    public static Action<int>   onStaminaChanged;
    public static Action<int>   onCoinsChanged;
    public static Action<int>   onGemsChanged;
    public static Action<float> onRewardProgressChanged;

    public static int stamina 
    { 
      get => get().economy.stamina; 
      set 
      {
        var _prev_val = get().economy.stamina;
        get().economy.stamina = Mathf.Clamp(value, 0, GameData.Econo.staminaMax);
        if(_prev_val != get().economy.stamina)
          onStaminaChanged?.Invoke(value);
      } 
    }
    public static int coins 
    { 
      get => get().economy.coins; 
      set
      { 
        var _prev_val = get().economy.coins;
        get().economy.coins = value;
        if(_prev_val != value)
          onCoinsChanged?.Invoke(value);
      }
    }
    public static int gems
    {
      get => get().economy.gems; 
      set 
      {
        var _prev_val = get().economy.gems;
        get().economy.gems = value;
        if(_prev_val != value)
          onGemsChanged?.Invoke(value);
      }
    }
    public static float rewards
    {
      get => get().economy.rewardPoints;
      set
      {
        var prev_points = get().economy.rewardPoints;
        get().economy.rewardPoints = value;
        if(prev_points != value)
          onRewardProgressChanged?.Invoke(value);
      }
    }
    public static bool CanSpendStamina(int stamina_cost)
    {
      return stamina >= stamina_cost;
    }
    public static class Chest
    {
      public static int      rewardLevel {get => get().economy.rewardLevel; set => get().economy.rewardLevel = value;}
      public static int      staminaCnt => get().economy.chestState.listStamina.Count;
      public static int      coinsCnt =>   get().economy.chestState.listCoins.Count;
      public static int      gemsCnt =>    get().economy.chestState.listGems.Count;
      public static Item.ID? PopRes() 
      {
        Item.ID? id = null;
        if(staminaCnt > 0)
        {
          id = get().economy.chestState.listStamina.last();
          get().economy.chestState.listStamina.RemoveAt(get().economy.chestState.listStamina.last_idx());
        }
        else if(coinsCnt > 0)
        {
          id = get().economy.chestState.listCoins.last();
          get().economy.chestState.listCoins.RemoveAt(get().economy.chestState.listCoins.last_idx());
        }
        else if(gemsCnt > 0)
        {
          id = get().economy.chestState.listGems.last();
          get().economy.chestState.listGems.RemoveAt(get().economy.chestState.listGems.last_idx());          
        }
        return id;
      }
      public static void  AddRewards()
      {
        get().economy.chestState.AddReward(GameData.Econo.GetRewards());
      }
    }
    public static float GetStaminaRefillPerc()
    {
      var now = CTime.get();
      var last = DateTime.FromBinary(get().economy.lastStamina);
      var timeDiff = now - last;
      float perc = 100*(float)((now - last).TotalSeconds / GameData.Econo.staminaRefillTime);
      
      return perc;
    }

    public static void Process()
    {
      var eco = get().economy;
      var now = CTime.get();
      var last = DateTime.FromBinary(eco.lastStamina);
      var timeDiff = now - last;
      if(timeDiff.TotalSeconds > GameData.Econo.staminaRefillTime)
      {
        int staminaToAdd = (int)(timeDiff.TotalSeconds / GameData.Econo.staminaRefillTime);
        stamina += staminaToAdd;
        eco.lastStamina = last.AddSeconds((double)staminaToAdd * GameData.Econo.staminaRefillTime).ToBinary();
      }
    }
  }
  public static class Settings
  {
    public static bool IsMuted {get => !get().settings.sounds; set{get().settings.sounds = !value;}}
  }
  public static class SplitMachine
  {
    public static int capacity {get => get().splitMachine.capacity; set => get().splitMachine.capacity = value;}
  }

  [Header("Customization")]
  [SerializeField] int selectedTheme = 0;
  public int SetNextTheme() {
    selectedTheme = (int)Mathf.Repeat(++selectedTheme, GameData.GetThemeColors().Length);
    return selectedTheme;
  }
}
