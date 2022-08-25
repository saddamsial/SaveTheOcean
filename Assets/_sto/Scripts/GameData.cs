
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib;


[CreateAssetMenu, DefaultExecutionOrder(-2)]
public class GameData : ScriptableObject
{
  private static GameData _static_this = null;
  public static  GameData get(){ return _static_this; }
  GameData()
  {
    _static_this = this;
  }

  public static void Init()
  {
    var items = get()._items;
    for(int type = 0; type < items.Length; ++type)
    {
      var item = items[type];
      for(int lvl = 0; lvl < item.Count; ++lvl)
      {
        item.Get(lvl).id = new Item.ID(type, lvl);
      }
    }
  }

  [System.Serializable]
  struct Items
  {
    [SerializeField] Item[] _items;
    public Item Get(int idx) => _items[Mathf.Clamp(idx, 0, _items.Length -1)];
    public int  Count => _items.Length;
  }

  [Header("Prefabs")]
  [SerializeField] Items[] _items;
  [SerializeField] GridTile _gridTile;
  [SerializeField] Location _locationPrefab;
  [Header("Levels")]
  [SerializeField] List<Level> _listLevels;
  [Header("Econo")]
  [SerializeField] int[] _rewardsToChest;


  [SerializeField] Color[]    themeColors;
  public static Color[] GetThemeColors() => get().themeColors;

  public static class Prefabs
  {
    public static GridTile CreateGridElem(Transform parent) 
    { 
      return Instantiate(get()._gridTile, parent); 
    }
    public static Item CreateItem(Item.ID id, Transform parent)
    {
      Item item = null;
      if(id.type < 0)
        id.type = Random.Range(0, ItemTypesCnt);
      item = Instantiate(get()._items[id.type].Get(id.lvl), parent);
      item.id = id;
      return item;
    }
    public static Item CreateStaticItem(Item.ID id, Transform parent)
    {
      Item item = CreateItem(id, parent);
      item.SetAsStatic();
      item.enabled = false;
      return item;
    }
    public static int ItemLevelsCnt(int item_type) => get()._items[item_type].Count;
    public static int ItemTypesCnt => get()._items.Length;

    public static Location CreateLocation(Transform parent) => Instantiate(get()._locationPrefab, parent);
  } 
  public static class Levels
  {
    static public Level GetPrefab(int idx) => get()._listLevels[idx];
    static public Level CreateLevel(int idx, Transform levelsContainer)
    {
      return Instantiate(get()._listLevels[idx], levelsContainer);
    }
    static public int   PrevLevel(int lvl_idx)
    {
      return (int)Mathf.Repeat(lvl_idx - 1.0f, get()._listLevels.Count);
    }
    static public int   NextLevel(int lvl_idx)
    {
      return (int)Mathf.Repeat(lvl_idx + 1.0f, get()._listLevels.Count);
    }
    static public int   LevelsCnt => get()._listLevels.Count;
  }
  public static class Econo
  {
    
  }

  public static class Settings
  {
  }
}