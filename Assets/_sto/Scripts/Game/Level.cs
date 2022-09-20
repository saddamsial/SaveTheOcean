using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib;
using GameLib.InputSystem;

using GarbCats = GameData.GarbCats;

public class Level : MonoBehaviour
{
  public static System.Action<Level>   onCreate, onStart, onTutorialStart, onGarbageOut, onNoRoomOnGrid, onItemHovered;
  public static System.Action<Level>   onDone, onFinished, onHide, onDestroy;
  public static System.Action<Vector3> onMagnetBeg;
  public static System.Action<bool>    onMagnetEnd;
  public static System.Action<Item>    onPremiumItem, onItemCollected;

  [Header("Refs")]
  [SerializeField] Transform      _itemsContainer;
  [SerializeField] Transform      _tilesContainer;
  //[SerializeField] Transform      _animalsContainer;
  [SerializeField] Transform[]    _animalContainers;
  [SerializeField] Renderer       _waterRenderer;
  [SerializeField] SplitMachine   _splitMachine;
  [SerializeField] FeedingMachine _feedingMachine;
  
  //[SerializeField] Transform[] _paths;
  //[SerializeField] Transform _poiLT;
  //[SerializeField] Transform _poiRB;

  [Header("Settings")]
  [SerializeField] Vector2Int _dim;
  [SerializeField] float      _gridSpace = 1.0f;
  [SerializeField] Color      _waterColor;
  [Header("LvlDesc")]
  [SerializeField] float[]    _chanceToDowngradeItem = new float[6];
  [SerializeField] int        _resItemPerItems = 0;
  [SerializeField] float      _resGemsPart = 0.1f;
  [SerializeField] float      _resCoinsPart = 0.4f;
  [SerializeField] float      _resStaminaPart = 0.5f;
  [SerializeField] LvlDesc[]  _lvlDescs;


  public enum State
  {
    Locked,
    Unlocked,
    Started,
    Finished,
    Polluted,
  }

  [System.Serializable]
  public struct LvlDesc
  {
    [SerializeField] Animal _animal;
    [SerializeField] GarbCats[] _itemsCats;

    public int GetSolutionMoveCount() {
      var solution = 0;
      for (int i = 0; i <_itemsCats.Length ; i++)
        solution += (int)Mathf.Pow(2, (int)_itemsCats[i]%10);
      return solution;
    } 

    public Animal  animal => _animal;
    public GarbCats[] itemsCats => _itemsCats;
    public Item items(int idx) => GameData.Prefabs.GetGarbagePrefab(_itemsCats[idx]);
  }

  public int GetNumberOfMovesToSolve(){
    var solution = 0;
    foreach (var animal in  _lvlDescs){
        solution += animal.GetSolutionMoveCount();
    }
    return solution;
  }

  public int    locationIdx {get; private set;} = -1;
  public bool   succeed {get; private set;}
  public bool   finished {get; private set;}
  public bool   wasPolluted {get; private set;} = false;
  public bool   wasFeeding => _isFeedingMode;
  public bool   isFeedingMode => _isFeedingMode;
  public int    points {get; set;} = 0;
  public int    stars {get; set;}
  public int    itemsCount => _items.Count + _items2.Count;
  public int    initialItemsCnt => _initialItemsCnt;
  public Vector2Int dim => (_isFeedingMode)? GameData.Levels.feedingDim : _dim;

  UISummary    _uiSummary = null;
  UIStatusBar  _uiStatusBar = null;
  List<Animal> _animals = new List<Animal>();
  MaterialPropertyBlock _mpb = null;

  Item        _itemSelected;
  Item        _itemHovered;
  Animal      _animalSelected;
  List<Item>  _items = new List<Item>();
  List<Item>  _items2 = new List<Item>();
  int         _requestCnt = 0;
  int         _initialItemsCnt = 0;

  //float       _pollutionRate = 1.0f;
  float       _pollutionDest = 1.0f;

  bool        _isFeedingMode = false;

  StorageBox _storageBox;

  public class Grid
  {
    Vector2Int  _dim;
    float       _gridSpace;
    int[,]      _grid;
    GridTile[,] _tiles;
    public void Init(Vector2Int dim, float grid_space)
    {
      _dim = dim;
      _gridSpace = grid_space;
      _grid = new int[dim.y, dim.x];
      _tiles = new GridTile[dim.y, dim.x];
      System.Array.Clear(_grid, 0, _grid.Length);
    }
    public static Vector2Int g2a(Vector2 vgrid, Vector2Int _dim)
    {
      int ax = (int)System.Math.Round(vgrid.x + _dim.x * 0.5f - 0.1f, System.MidpointRounding.AwayFromZero);
      int ay = (int)System.Math.Round(vgrid.y + _dim.y * 0.5f - 0.1f, System.MidpointRounding.AwayFromZero);
      return new Vector2Int(ax, ay);
    }
    public static Vector2 a2g(Vector2Int va, Vector2Int _dim)
    {
      Vector2 v = Vector2.zero;
      v.y = ((-_dim.y + 1) * 0.5f + va.y);
      v.x = (-_dim.x + 1) * 0.5f + va.x;
      return v;
    }
    public void set(Vector2 vgrid, int val, Item.Kind kind = Item.Kind.None)
    {
      var va = g2a(vgrid, _dim);
      _grid[va.y, va.x] = val;
      _tiles[va.y, va.x].Set((val!=0)? true : false, kind == Item.Kind.Garbage);
    }
    public int get(Vector2 vgrid)
    {
      var va = g2a(vgrid, _dim);
      return _grid[va.y, va.x];
    }
    public GridTile getTile(Vector2 vgrid)
    {
      var va = g2a(vgrid, _dim);
      return _tiles[va.y, va.x];
    }
    public void tile(GridTile gt, Vector2 vgrid)
    {
      var va = g2a(vgrid, _dim);
      _tiles[va.y, va.x] = gt;
      gt.vgrid = vgrid;
      gt.Set(false);
    }
    public void hovers(bool hov)
    {
      for(int y = 0; y < _dim.y; ++y)
      {
        for(int x = 0; x < _dim.x; ++x)
        {
          _tiles[y,x].Hover(hov);
        }
      }
    }
    public Vector2? getEmpty()
    {
      List<Vector2> vps = new List<Vector2>();
      for(int y = 0; y < _dim.y; ++y)
      {
        for(int x = 0; x < _dim.x; ++x)
        {
          if(_grid[y, x] == 0)
            vps.Add(a2g(new Vector2Int(x, y), _dim));
        }
      }
      return (vps.Count > 0)? vps.get_random() : null;
    }
    public bool isInside(Vector2 vgrid) => Mathf.Abs(vgrid.x * 2) <= _dim.x && Mathf.Abs(vgrid.y*2) <= _dim.y;
    public bool isOverAxisZ(Vector3 vpos)
    {
      Vector2 vdim = new Vector2(_dim.x, _dim.y) * _gridSpace;
      return vpos.z <= vdim.y / 2;
    }
    public float getMaxZ()
    {
      return _dim.y * 0.5f * _gridSpace;
    }
  }

  Grid _grid = new Grid();

  void Awake()
  {
    locationIdx = GameState.Progress.locationIdx;

    Item.gridSpace = _gridSpace;
    _uiSummary = FindObjectOfType<UISummary>(true);
    _uiStatusBar = FindObjectOfType<UIStatusBar>(true); 

    _mpb = new MaterialPropertyBlock();
    _mpb.SetColor("_BaseColor", _waterColor);
    _waterRenderer.SetPropertyBlock(_mpb);

    _isFeedingMode = GameState.Progress.Locations.IsLocationFinished(locationIdx);
    wasPolluted = GameState.Progress.Locations.GetLocationState(locationIdx) == Level.State.Polluted;

    _splitMachine.Init(_items);
    _splitMachine.gameObject.SetActive(!_isFeedingMode);
    _feedingMachine.gameObject.SetActive(_isFeedingMode);

    _storageBox = GetComponentInChildren<StorageBox>();

    onCreate?.Invoke(this);
  }
  void OnDestroy()
  {
    onDestroy?.Invoke(this);
  }
  IEnumerator Start()
  {
    Init();
    yield return null;

    for(int q = 0; q < _lvlDescs.Length; ++q)
    {
      var animal = Instantiate(_lvlDescs[q].animal, _animalContainers[q]);
      animal.Init(_lvlDescs[q].itemsCats);
      animal.Activate(true);
      _animals.Add(animal);
    }

    onStart?.Invoke(this);
    CheckMatchingItems();
  }
  public void Hide()
  {
    onHide?.Invoke(this);
  }
  void  Init()
  {
    List<int> levels_idx = new List<int>();
    levels_idx.Capacity = 1000;
    for(int q = 0; q < _chanceToDowngradeItem.Length; ++q)
    {
      int cnt = (int)(1000 * _chanceToDowngradeItem[q]);
      for(int w = 0; w < cnt; ++w)
        levels_idx.Add(q);
    }
    levels_idx.shuffle(2000);

    _grid.Init(dim, _gridSpace);

    List<Vector2> vs = new List<Vector2>();
    Vector2 v = Vector2.zero;
    for(int y = 0; y < dim.y; ++y)
    {
      v.y = -((-dim.y + 1) * 0.5f + y);
      for(int x = 0; x < dim.x; ++x)
      {
        v.x = (-dim.x + 1) * 0.5f + x;
        vs.Add(v);
        var tile = GameData.Prefabs.CreateGridElem(_tilesContainer);
        tile.transform.localPosition = Item.ToPos(v);
        _grid.tile(tile, v);
      }
    }
    vs.shuffle(100);
    vs.Reverse();
    if(!_isFeedingMode)
    {
      Item.ID id = new Item.ID();
      List<Item.ID> ids = new List<Item.ID>();
      for(int q = 0; q < _lvlDescs.Length; ++q)
      {
        var lvlDesc = _lvlDescs[q];
        _requestCnt += lvlDesc.itemsCats.Length;
        for(int i = 0; i < lvlDesc.itemsCats.Length; ++i)
        {
          var item = _lvlDescs[q].items(i);
          int itemLevel = item.id.lvl;
          id.type = item.id.type;
          id.kind = item.id.kind;
          id.lvl = item.id.lvl;
          int vi = (levels_idx.Count > 0)? levels_idx[Random.Range(0, levels_idx.Count-1)] : 0;
          if(vi < itemLevel)
          {
            for(int d = 0; d < 1 << (itemLevel-vi); ++d)
            {
              id.lvl = vi;
              ids.Add(id);
            }
          }
          else
          {
            ids.Add(id);
          }
        }
      }
      if(_resItemPerItems > 0)
      {
        int resItems = ids.Count / _resItemPerItems;
        var extras = new (Item.Kind kind, float weight)[]
        {
          new (Item.Kind.Stamina, _resStaminaPart),
          new (Item.Kind.Coin, _resCoinsPart),
          new (Item.Kind.Gem, _resGemsPart),
        };

        for(int q = 0; q < extras.Length; ++q)
        {
          int cnt = Mathf.RoundToInt(resItems * extras[q].weight);
          for(int i = 0; i < cnt ; ++i)
          {
            var spec_id = new Item.ID(0, 0, extras[q].kind);
            ids.Add(spec_id);
          } 
        }
      }
      ids.shuffle(500);

      for(int q = 0; q < ids.Count; ++q)
      {
        var item = GameData.Prefabs.CreateItem(ids[q], _itemsContainer);
        if(vs.Count > 0)
        {
          item.Init(vs.first());
          vs.RemoveAt(0);
          item.Spawn(item.vgrid, null, 15, Random.Range(0.5f, 1.5f));
          AddItem(item);
        }
        else
        {
          item.Init(Vector2.zero);
          _items2.Add(item);
          item.gameObject.SetActive(false);
        }
      }

    }
    else //feeding
    {
      for(int q = 0; q < GameState.Feeding.FoodCnt; ++q)
      {
        var food = GameState.Feeding.GetFood(q);
        var item = GameData.Prefabs.CreateItem(food.id, _itemsContainer);
        item.Init(food.vgrid);
        item.Spawn(item.vgrid, null, 15, Random.Range(0.5f, 1.5f));
        AddItem(item);
      }
    }
    _initialItemsCnt = itemsCount;
  }
  bool  firstPremium = false;
  void  AddItem(Item item)
  {
    _items.Add(item);
    _grid.set(item.vgrid, 1, item.id.kind);
    if(!GameState.Tutorial.premiumDone && !firstPremium && item.id.IsSpecial)
    {
      firstPremium = true;
      onPremiumItem?.Invoke(item);
    }
  }
  void  SpawnItem(Vector2 vgrid)
  {
    if(_items2.Count > 0)
    {
      var item = _items2.first();
      _items2.RemoveAt(0);
      item.Spawn(vgrid, null, 15, 1);
      AddItem(item);
    }
  }
  void MoveItemBack(Item item)
  {
    item.Select(false);
    item.MoveBack();
    if(!item.IsInMachine)
      _grid.set(item.vgrid, 1, item.id.kind);
  }  
  void  DestroyItem(Item item)
  {
    _items.Remove(item);
    _grid.set(item.vgrid, 0);
    item.Hide();
  }

  public float PollutionRate()
  {
    int requests = 0;
    _animals.ForEach((animal) => requests += animal.requests);
    return (float)requests / _requestCnt;
  }

  public void OnInputBeg(TouchInputData tid)
  {
    _itemSelected = null;
    _itemSelected = tid.GetClosestCollider(0.5f, Item.layerMask)?.GetComponent<Item>() ?? null;
    _itemSelected?.Select(true);
    _itemHovered = null;
    voffs = Vector3.zero;
  }
  Vector3 voffs = Vector3.zero;
  public void OnInputMov(TouchInputData tid)
  {
    if(finished)
      return;
    Item nearestItem = null;
    Animal nearestAnimal = null;
    if(_itemSelected && tid.RaycastData.HasValue)
    {
      var vpt = tid.RaycastData.Value.point;
      if(_grid.isOverAxisZ(tid.RaycastData.Value.point))
        voffs.y = Mathf.Lerp(voffs.y, 0.6f, Time.deltaTime * 10);
      else
        voffs.y = Mathf.Clamp(1 + 0.20f * (vpt.z - _grid.getMaxZ()), 0, 2.0f);
      _itemSelected.vwpos = Vector3.Lerp(_itemSelected.vwpos, vpt + voffs + _itemSelected.vbtmExtent, Time.deltaTime * 20);

      var _nearestHit = tid.GetClosestCollider(0.5f, Item.layerMask | Animal.layerMask);//?.GetComponent<Item>() ?? null;
      nearestItem = _nearestHit?.GetComponent<Item>();
      if(nearestItem)
      {
        nearestItem.Hover(true);
        if(nearestItem != _itemHovered)
          onItemHovered?.Invoke(this);
      }
      _itemHovered = nearestItem;

      nearestAnimal = _nearestHit?.GetComponent<Animal>();
      if(nearestAnimal && _animalSelected == null)
      {
        _animalSelected = nearestAnimal;
        if(!_isFeedingMode)
          _animalSelected.AnimTalk();
      }
      else
        _animalSelected = null;
    }

    _grid.hovers(false);
    if(_itemSelected)
    {
      if(nearestItem)
      {
        _grid.getTile(nearestItem.vgrid).Hover(true);
      }
      else  
      {
        var tileHit = tid.GetClosestObjectInRange<GridTile>(0.5f);
        tileHit?.Hover(true);
      }
    }
    if(nearestAnimal && nearestAnimal.CanPut(_itemSelected))
      onMagnetBeg?.Invoke(nearestAnimal.transform.position);
    else if(nearestItem != null && Item.Mergeable(_itemSelected, nearestItem))
      onMagnetBeg?.Invoke(nearestItem.transform.position);
    else
      onMagnetEnd?.Invoke(false);
  }
  public void OnInputEnd(TouchInputData tid)
  {
    if(!_itemSelected)
      return;

    bool is_hit = IsItemHit(tid) || IsAnimalHit(tid) || IsTileHit(tid) || IsSplitMachineHit(tid) || IsStorageHit(tid) || IsChestHit(tid);
    if(!is_hit)
    {
      MoveItemBack(_itemSelected);
    }
    _itemSelected = null;
    _grid.hovers(false);
    _itemHovered = null;
    CheckMatchingItems();
    onMagnetEnd?.Invoke(false);
  }
  double tapTime = 0;
  public void OnInputTapped(TouchInputData tid)
  {
    int layers = RewardChest.layerMask | StorageBox.layerMask;
    if(_feedingMachine.gameObject.activeInHierarchy)
      layers |= FeedingMachine.layerMask;
    var box = tid.GetClosestCollider(0.5f, layers);
    if(box)
    {
      //if(Time.timeAsDouble - tapTime < 1.0f)
      {
        tapTime = 0;
        Vector2? vg = _grid.getEmpty();
        Vector3  vbeg = Vector3.zero;
        if(vg != null)
        {
          var chest = box.GetComponent<RewardChest>();
          Item.ID? id = null;
          if(chest)
          {
            id = chest.Pop();
            vbeg = chest.transform.position;
          }
          else
          {
            var storage = box.GetComponent<StorageBox>();
            if(storage)
            {
              id = storage.Pop();
              vbeg = storage.transform.position;
            }
            else
            {
              var feeding = box.GetComponent<FeedingMachine>();
              if(feeding)
              {
                id = feeding.Pop();
                vbeg = feeding.vpos;
              }
            }
          }
          if(id != null)
          {
            var item = GameData.Prefabs.CreateItem(id.Value, _itemsContainer);
            item.vgrid = vg.Value;
            AddItem(item);
            item.Throw(vbeg, item.vgrid);
            GameState.Feeding.Update(_items);
          }
        }
        else
          onNoRoomOnGrid?.Invoke(this);
      }
      //else
      //  tapTime = Time.timeAsDouble;
    }
    else
    {
      var item = tid.GetClosestCollider(0.5f, Item.layerMask)?.GetComponent<Item>();
      if(item && item.id.IsSpecial)
      {
        if(Time.timeAsDouble - tapTime < 1.0f)
        {
          tapTime = 0;
          int amount = GameState.Econo.AddRes(item.id);
          _items.Remove(item);
          _grid.set(item.vgrid, 0);
          _uiStatusBar.MoveCollected(item, amount);
          onItemCollected?.Invoke(item);
          item.Hide();
          SpawnItem(item.vgrid);
        }
        else
          tapTime = Time.timeAsDouble;
      }
    }
  }
  bool IsItemHit(TouchInputData tid)
  {
    bool is_hit = false;
    var itemHit = tid.GetClosestCollider(0.5f, Item.layerMask)?.GetComponent<Item>() ?? null;
    bool is_merged = false;
    if(itemHit && itemHit != _itemSelected && !itemHit.IsInMachine)
    {
      is_hit = true;
      var newItem = Item.Merge(_itemSelected, itemHit, _items);
      if(newItem)
      {
        _grid.set(_itemSelected.vgrid, 0);
        _splitMachine.RemoveFromSplitSlot(_itemSelected);
        newItem.Show();
        SpawnItem(_itemSelected.vgrid);
        is_merged = true;
        if(_isFeedingMode)
          GameState.Feeding.Update(_items);
      }
    }
    if(is_hit && !is_merged)
    {
      MoveItemBack(_itemSelected);
    }

    return is_hit;
  }
  bool IsAnimalHit(TouchInputData tid)
  {
    bool is_hit = false;
    var animalHit = tid.GetClosestCollider(0.5f, Animal.layerMask)?.GetComponent<Animal>() ?? null;
    if(animalHit)
    {
      if(animalHit.IsReq(_itemSelected)) //CanPut(_itemSelected))
      {
        Item.onPut?.Invoke(_itemSelected);
        animalHit.Put(_itemSelected, _isFeedingMode);
        _grid.set(_itemSelected.vgrid, 0);
        _items.Remove(_itemSelected);
        if(_itemSelected.IsInMachine)
          _splitMachine.RemoveFromSplitSlot(_itemSelected);

        _pollutionDest = PollutionRate();
        onGarbageOut?.Invoke(this);
        SpawnItem(_itemSelected.vgrid);
        CheckEnd();
        if(_isFeedingMode)
          GameState.Feeding.Update(_items);
        is_hit = true;
      }
      else
      {
        if(!animalHit.IsReq(_itemSelected))
          Item.onNoPut?.Invoke(_itemSelected);
      }
    }

    return is_hit;
  }
  bool IsSplitMachineHit(TouchInputData tid)
  {
    bool is_hit = false;
    var splitMachineHit = tid.GetClosestCollider(0.5f);
    bool is_split_machine = _splitMachine?.IsDropSlot(splitMachineHit) ?? false;
    if(is_split_machine)
    {
      bool itemFromSplitMachine = _itemSelected.IsInMachine && _splitMachine.capacity == 1;
      if(_splitMachine.IsReady || itemFromSplitMachine)
      {
        if(_itemSelected.IsSplitable && !_itemSelected.id.IsSpecial)
        {
          if(itemFromSplitMachine)
            _splitMachine.RemoveFromSplitSlot(_itemSelected);
          _splitMachine.DropDone();
          _grid.set(_itemSelected.vgrid, 0);
          _splitMachine.AddToDropSlot(_itemSelected);
          is_hit = true;
        }
        else
          _splitMachine.DropNoSplittable();
      }
      else
        _splitMachine.DropNoCapacity();

      SplitMachine.onDropped?.Invoke(_splitMachine);
    }
    return is_hit;
  }
  bool IsTileHit(TouchInputData tid)
  {
    bool is_hit = false;
    //if(_itemSelected.IsInMachine)
    {
      var tileHit = tid.GetClosestObjectInRange<GridTile>(0.5f);
      if(tileHit && _grid.get(tileHit.vgrid) == 0)
      {
        _grid.set(_itemSelected.vgrid, 0);
        _itemSelected.vgrid = tileHit.vgrid;
        _grid.set(_itemSelected.vgrid, 1, _itemSelected.id.kind);
        _itemSelected.Select(false);
        _splitMachine.RemoveFromSplitSlot(_itemSelected);
        _itemSelected.MoveToGrid();
        is_hit = true;
        _grid.hovers(false);
        if(_isFeedingMode)
          GameState.Feeding.Update(_items);
      }
    }
    return is_hit;
  }
  bool IsStorageHit(TouchInputData tid)
  {
    bool is_hit = false;
    var storage = tid.GetClosestObjectInRange<StorageBox>(0.5f, StorageBox.layerMask);
    if(storage)
    {
      if(_itemSelected.id.IsSpecial)
      {
        storage.Push(_itemSelected.id);
        _items.Remove(_itemSelected);
        _grid.set(_itemSelected.vgrid, 0);
        _itemSelected.Hide();
        is_hit = true;
        SpawnItem(_itemSelected.vgrid);
      }
      else
        storage.NoPush(_itemSelected.id);
    }

    return is_hit;
  }
  bool IsChestHit(TouchInputData tid)
  {
    bool is_hit = false;
    var chest = tid.GetClosestObjectInRange<RewardChest>(0.5f, RewardChest.layerMask);
    if(chest)
      chest.NoPush(_itemSelected.id);
    
    return is_hit;
  }
  public void End()
  {
    Item[] itms = _items.FindAll((Item item) => item.id.IsSpecial).ToArray();
    foreach(var itm in itms)
    {
      _storageBox.Push(itm.id.Validate(true));
      DestroyItem(itm);
    }
  }
  IEnumerator coMoveToSB()
  {
    Vector3 vsbox = _storageBox.transform.position + new Vector3(0, 1, 0);
    List<Item> itms = _items.FindAll((Item item) => item.id.IsSpecial).ToList();
    while(itms.Count > 0)
    {
      for(int q = 0; q < itms.Count;)
      {
        itms[q].vwpos = Vector3.Lerp(itms[q].vwpos, vsbox, Time.deltaTime * 4);
        if(Vector3.Distance(itms[q].vwpos, vsbox) < 0.1f)
        {
          _storageBox.Push(itms[q].id.Validate(true));
          DestroyItem(itms[q]);
          itms.RemoveAt(q);
          --q;
        }
        ++q;
      }
      yield return null;
    }
  }
  IEnumerator coEnd()
  {
    if(!GameState.Tutorial.storageDone && _items.Any((item) => item.id.IsSpecial))
    {
      _storageBox.Show(0.05f);
      yield return new WaitForSeconds(1.0f);
    }
    yield return StartCoroutine(coMoveToSB());

    yield return new WaitForSeconds(2.5f);
    succeed = true;
    onFinished?.Invoke(this);
    GameState.Progress.Locations.SetLocationFinished();
    GameState.Progress.Locations.UnlockNextLocation();
    yield return new WaitForSeconds(0.5f);

    _uiSummary.Show(this);
  }
  void CheckEnd()
  {
    int activeAnimals = _animals.Count((animal) => animal.isActive);
    if(!finished && activeAnimals == 0)
    {
      finished = true;
      StartCoroutine(coEnd());
    }
  }
  void CheckMatchingItems()
  {
    List<Item> req_items = new List<Item>();
    _animals.ForEach((anim) => req_items.AddRange(anim.garbages));
    req_items.ForEach((reqit) => reqit.tickIco = _items.Any((it) => Item.EqType(it, reqit)));
  }
  void Process()
  {
    foreach(var _item in _items)
    {
      if(!_item.IsSelected)
        _item.Hover(false);
    }
  }
  void Update()
  {
    Process();

  #if UNITY_EDITOR
    if(Input.GetKeyDown(KeyCode.E))
    {
      if(!finished)
      {
        finished = true;
        StartCoroutine(coEnd());
      }
    }
  #endif     
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Vector3 vLB = new Vector3(-dim.x * 0.5f, 0, -dim.y * 0.5f);
    Vector3 vRT = new Vector3( dim.x * 0.5f, 0, dim.y * 0.5f);
    var v1 = Vector3.zero;
    v1.x = vLB.x;
    v1.z = vRT.z;
    Gizmos.DrawLine(vLB, v1);
    v1.x = vRT.x;
    v1.z = vLB.z;
    Gizmos.DrawLine(vLB, v1);
    v1.x = vRT.x;
    v1.z = vLB.z;
    Gizmos.DrawLine(vRT, v1);
    v1.x = vLB.x;
    v1.z = vRT.z;
    Gizmos.DrawLine(vRT, v1);
  }
}
