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
    GameState.Econo.Process();
    GameState.Progress.Locations.Process();
  }

  [System.Serializable]
  class LocationState
  {
    [SerializeField] int _idx = 0;
    [SerializeField] Level.State _state = Level.State.Locked;
    [SerializeField] long _date = 0;

    public LocationState(int loc_idx, Level.State st = Level.State.Locked)
    {
      _idx = loc_idx;
      _state = st;
    }
    public int idx => _idx;
    public Level.State state { get => _state; set => _state = value;}
    public long date {get => _date; set => _date = value;}
  }

  [System.Serializable]
  class ProgressState
  {
    [SerializeField] int _location = 0;
    [SerializeField] List<LocationState> _locations;
    [SerializeField] long _locationsPassedTime = 0;

    public static Action onAllLocationFinished;

    public  int  location { get => _location; set => _location = value; }
    public  long locationsPassTime { get => _locationsPassedTime; set => _locationsPassedTime = value;}
    public  List<LocationState> locations { get => _locations; }
    public  LocationState  FindLocation(int loc_idx)
    {
      return _locations.Find((loc) => loc.idx == loc_idx);
    }
    public  bool        IsLocationUnlocked(int loc_idx)
    {
      var loc = FindLocation(loc_idx);
      return (loc != null)? loc.state >= Level.State.Unlocked : false;
    }
    public  bool        IsLocationPassed(int loc_idx)
    {
      var loc = FindLocation(loc_idx);
      return (loc != null) ? loc.state == Level.State.Finished : false;
    }
    public  void        UnlockLocation(int loc_idx)
    {
      var loc = FindLocation(loc_idx);
      if(loc == null)
        _locations.Add(new LocationState(loc_idx, Level.State.Unlocked));
      else
      {
        if(loc.state == Level.State.Locked)
          loc.state = Level.State.Unlocked;
      }
    }
    public void         PassLocation(int loc_idx)
    {
      var loc = FindLocation(loc_idx);
      loc.date = CTime.get().ToBinary();
      if(loc == null)
        _locations.Add(new LocationState(loc_idx, Level.State.Finished));
      else
        loc.state = Level.State.Finished;

      if(_locations.Count == Earth.locationsCnt)
      {
        if(_locationsPassedTime == 0)
        {
          _locationsPassedTime = CTime.get().ToBinary();
          onAllLocationFinished?.Invoke();
        }
      }
    }
    public Level.State  GetLocationState(int loc_idx)
    {
      var loc = FindLocation(loc_idx);
      return (loc != null) ? loc.state : Level.State.Locked;
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
  }
  [SerializeField] EconomyState economy;

  [System.Serializable]
  class ChestState
  {
    public List<Item.ID> listStamina = new List<Item.ID>();
    public List<Item.ID> listCoins = new List<Item.ID>();
    public List<Item.ID> listGems = new List<Item.ID>();

    public void AddReward(GameData.Rewards.Reward rew)
    {
      listStamina.Add(new Item.ID(0, 0, Item.Kind.Stamina));
      listCoins.Add(new Item.ID(0, 0, Item.Kind.Coin));
      listGems.Add(new Item.ID(0, 0, Item.Kind.Gem));
    }
  }
  [SerializeField] ChestState chest;

  [System.Serializable]
  class StorageState
  {
    public List<Item.ID> listItems;
  };
  [SerializeField] StorageState storage;

  [System.Serializable]
  class SplitMachineState
  {
    [SerializeField] int _capacity = 1;

    public int capacity {get => _capacity ; set => _capacity = value;}
  }
  [SerializeField] SplitMachineState splitMachine;

  [System.Serializable]
  class FeedingState
  {
    [System.Serializable]
    public struct Food
    {
      public Item.ID _id;
      public Vector2 _vgrid;
      public Food(Item item)
      {
        _id = item.id;
        _vgrid = item.vgrid;
      }
    }
    public List<Food> foods = new List<Food>();
  }
  [SerializeField] FeedingState feeding;

  [System.Serializable]
  class GameInfoState
  {
    public long appQuitTime;
  }
  [SerializeField] GameInfoState gameInfo;

  public static void Init()
  {
    get().progress.UnlockLocation(0);
    if(get().economy.lastStamina == 0)
      get().economy.lastStamina = CTime.get().ToBinary();
  }

  public static class Progress
  {
    public static int locationIdx {get => get().progress.location; set => get().progress.location = value;}
    public static class Locations
    {
      public static Action<int>   onLocationPolluted;  

      public static Level.State   GetLocationState(int loc_idx) => get().progress.GetLocationState(loc_idx);
      //public static void        SetLocationState(int loc_idx, Location.State state) => get().progress.SetLocationState(loc_idx, state);
      public static bool          IsLocationUnlocked(int loc_idx) => get().progress.IsLocationUnlocked(loc_idx);
      public static bool          IsLocationFinished(int loc_idx) => get().progress.IsLocationPassed(loc_idx);
      public static void          SetLocationFinished(int loc_idx) => get().progress.PassLocation(loc_idx);
      public static void          SetLocationFinished() => SetLocationFinished(locationIdx);
      //public static void          UnlockNextLocation(int loc_idx) => get().progress.UnlockLocation(GameData.Levels.NextLocation(loc_idx));
      public static void          UnlockNextLocation(int loc_idx) => get().progress.UnlockLocation(GameData.Locations.NextLocation(loc_idx));
      public static void          UnlockNextLocation() => UnlockNextLocation(locationIdx);
      public static Level.State[] GetStates()
      {
        var states = new Level.State[Earth.locationsCnt];
        for(int q = 0; q < states.Length; q++)
          states[q] = GetLocationState(q);

        return states;
      }
      public static bool          AllStateFinished() => GetStates().All((state) => state >= Level.State.Finished);
      public static bool          AllStateFinished(Level.State[] states) => states.All((state) => state >= Level.State.Finished);

      static int _timer = 0;
      static DateTime prevTime;
      public static void Process()
      {
        if(get().progress.locationsPassTime != 0 && ++_timer % 60 == 0)
        {
          if(DateTime.FromBinary(GameInfo.appQuitTime) > prevTime)
            prevTime = DateTime.FromBinary(GameInfo.appQuitTime);

          var locs = get().progress.locations.FindAll((loc) => loc.state == Level.State.Finished);
          locs.shuffle(100);
          locs.Reverse();
          for(int q = 0; q < SystemNotificationOnTimeEx.dateTimes.Count && locs.Count > 0; ++q)
          {
            if(prevTime < SystemNotificationOnTimeEx.dateTimes[q] && CTime.get() > SystemNotificationOnTimeEx.dateTimes[q])
            {
              locs.first().state = Level.State.Warning;
              onLocationPolluted?.Invoke(locs.first().idx);
              locs.RemoveAt(0);
            }
          }
          prevTime = CTime.get();
        }
      }
    }

    public static float GetCompletionRate()
    {
      Level.State[] states = Locations.GetStates();
      float finishedCnt = states.Where((state) => state == Level.State.Finished).Count();
      return finishedCnt / Earth.locationsCnt;
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
        get().economy.coins = Mathf.Clamp(value, 0, GameData.Econo.coinsMax);
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
        get().economy.gems = Mathf.Clamp(value, 0, GameData.Econo.gemsMax);
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
    public static bool CanSpendStamina(int stamina_cost) => stamina >= stamina_cost;
    public static bool CanSpendCoins(int coins_cost) => coins >= coins_cost;
    
    public static float GetStaminaRefillPerc()
    {
      var now = CTime.get();
      var last = DateTime.FromBinary(get().economy.lastStamina);
      var timeDiff = now - last;
      float perc = 100*(float)((now - last).TotalSeconds / GameData.Econo.staminaRefillTime);
      
      return perc;
    }
    public static int AddRes(Item.ID id) //without event
    {
      int amount = (int)((1 << id.lvl) * 1.5f);
      if(id.kind == Item.Kind.Stamina)
        get().economy.stamina = Mathf.Clamp(get().economy.stamina + amount, 0, GameData.Econo.staminaMax);
      else if(id.kind == Item.Kind.Coin)
        get().economy.coins = Mathf.Clamp(get().economy.coins + amount, 0, GameData.Econo.coinsMax);
      else if(id.kind == Item.Kind.Gem)
        get().economy.gems = Mathf.Clamp(get().economy.gems + amount, 0, GameData.Econo.gemsMax);

      return amount;  
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
  public static class SplitMachine
  {
    public static int capacity {get => get().splitMachine.capacity; set => get().splitMachine.capacity = value;}
  }
  public static class Chest
  {
    public static int rewardLevel { get => get().economy.rewardLevel; set => get().economy.rewardLevel = value; }
    public static int staminaCnt => get().chest.listStamina.Count;
    public static int coinsCnt => get().chest.listCoins.Count;
    public static int gemsCnt => get().chest.listGems.Count;
    public static Item.ID? PopRes()
    {
      Item.ID? id = null;
      if(staminaCnt > 0)
      {
        id = get().chest.listStamina.last().Validate(true);
        get().chest.listStamina.RemoveAt(get().chest.listStamina.last_idx());
      }
      else if(coinsCnt > 0)
      {
        id = get().chest.listCoins.last().Validate(true);
        get().chest.listCoins.RemoveAt(get().chest.listCoins.last_idx());
      }
      else if(gemsCnt > 0)
      {
        id = get().chest.listGems.last().Validate(true);
        get().chest.listGems.RemoveAt(get().chest.listGems.last_idx());
      }
      return id;
    }
    public static void AddRewards()
    {
      get().chest.AddReward(GameData.Econo.GetRewards());
    }
  }
  public static class StorageBox
  {
    public static Action onItemsCntChanged;
    
    public static void PushItem(Item.ID id)
    {
      get().storage.listItems.Add(id);
    }
    public static Item.ID TopItem()
    {
      Item.ID id = new Item.ID(0, 0, Item.Kind.None);
      if(get().storage.listItems.Count > 0)
        id = get().storage.listItems[get().storage.listItems.last_idx()];
      return id;
    }
    public static Item.ID? PopItem()
    {
      Item.ID? id = null;
      if(get().storage.listItems.Count > 0)
      {
        id = TopItem();
        get().storage.listItems.RemoveAt(get().storage.listItems.last_idx());
      }
      return id;
    }
    public static int ItemsCnt() => get().storage.listItems.Count;
  }
  public static class Feeding
  {
    public static void Update(List<Item> items)
    {
      get().feeding.foods.Clear();
      for(int q = 0; q < items.Count; ++q)
      {
        if(items[q].id.kind == Item.Kind.Food)
          get().feeding.foods.Add(new FeedingState.Food(items[q]));
      }
    }
    public static int   FoodCnt => get().feeding.foods.Count;
    public static (Item.ID id, Vector2 vgrid) GetFood(int idx) => new (get().feeding.foods[idx]._id, get().feeding.foods[idx]._vgrid);
  }
  public static class GameInfo
  {
    public static long appQuitTime {get => get().gameInfo.appQuitTime; set => get().gameInfo.appQuitTime = value;}
  }

  [Header("Customization")]
  [SerializeField] int selectedTheme = 0;
  public int SetNextTheme() {
    selectedTheme = (int)Mathf.Repeat(++selectedTheme, GameData.GetThemeColors().Length);
    return selectedTheme;
  }
}
