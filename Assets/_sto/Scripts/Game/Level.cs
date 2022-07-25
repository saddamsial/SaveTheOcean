using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib;
using GameLib.CameraSystem;
using GameLib.Splines;
using GameLib.InputSystem;
using GameLib.Utilities;
using TMPLbl = TMPro.TextMeshPro;

public class Level : MonoBehaviour
{
  public static System.Action<Level>   onCreate, onStart, onTutorialStart;
  public static System.Action<Level>   onDone, onFinished, onDestroy;

  [Header("Refs")]
  [SerializeField] Transform _itemsContainer;
  [SerializeField] Transform _gridContainer;
  [SerializeField] Transform _animalsContainer;
  [SerializeField] Transform[] _animalContainers;
  //[SerializeField] Transform _poiLT;
  //[SerializeField] Transform _poiRB;

  [Header("Settings")]
  [SerializeField] Vector2Int _dim;
  [SerializeField] float      _gridSpace = 1.0f;
  [Header("LvlDesc")]
  //[SerializeField] List<Item> _listItems;
  [SerializeField] LvlDesc[]  _lvlDescs;

  [System.Serializable]
  public struct LvlDesc
  {
    [SerializeField] Animal _animal;
    [SerializeField] Item[] _reqItems;

    public Animal  animal => _animal;
    public Item[]  items => _reqItems;
  }

  public int    LevelIdx => GameState.Progress.Level;
  public bool   Succeed {get; private set;}
  public bool   Finished {get; private set;}
  public int    Points {get; set;} = 0;
  public int    Stars {get; set;}
  public Vector2Int Dim => _dim;


  bool         _started = false;
  UISummary    _uiSummary = null;
  Transform    _cameraContainer = null;
  List<Animal> _animals = new List<Animal>();

  Item       _itemSelected;
  List<Item> _items = new List<Item>();
  List<Item> _items2 = new List<Item>();

  public class Grid
  {
    Vector2Int  _dim;
    int[,]      _grid;
    GridTile[,] _tiles;
    public void Init(Vector2Int dim)
    {
      _dim = dim;
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
      v.y = -((-_dim.y + 1) * 0.5f + va.y);
      v.x = (-_dim.x + 1) * 0.5f + va.x;
      return v;
    }
    public void set(Vector2 vgrid, int val)
    {
      var va = g2a(vgrid, _dim);
      _grid[va.y, va.x] = val;
      _tiles[va.y, va.x].set((val!=0)? true : false);
    }
    public int get(Vector2 vgrid)
    {
      var va = g2a(vgrid, _dim);
      return _grid[va.y, va.x];
    }
    public void tile(GridTile gt, Vector2 vgrid)
    {
      var va = g2a(vgrid, _dim);
      _tiles[va.y, va.x] = gt;
      gt.set(false);
    }
  }

  Grid _grid = new Grid();

  void Awake()
  {
    Item.GridSpace = _gridSpace;
    _cameraContainer = GameObject.Find("_cameraContainer").transform;
    _items = _itemsContainer.GetComponentsInChildren<Item>().ToList();
    _uiSummary = FindObjectOfType<UISummary>(true);
    //_animals = _animalsContainer.GetComponentsInChildren<Animal>();

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
      animal.Init(_lvlDescs[q].items);
      animal.Activate(true);
      _animals.Add(animal);
    }

    _started = true;
    onStart?.Invoke(this);
  }
  void Init()
  {
    _grid.Init(_dim);

    List<Vector2> vs = new List<Vector2>();
    Vector2 v = Vector2.zero;
    for(int y = 0; y < _dim.y; ++y)
    {
      v.y = -((-_dim.y + 1) * 0.5f + y);
      for(int x = 0; x < _dim.x; ++x)
      {
        v.x = (-_dim.x + 1) * 0.5f + x;
        vs.Add(v);
        var tile = GameData.Prefabs.CreateGridElem(_gridContainer);
        tile.transform.localPosition = Item.ToPos(v);
        _grid.tile(tile, v);
      }
    }
    Item.ID id = new Item.ID();
    List<Item.ID> ids = new List<Item.ID>();
    for(int q = 0; q < _lvlDescs.Length; ++q)
    {
      var lvlDesc = _lvlDescs[q];
      for(int i = 0; i < lvlDesc.items.Length; ++i)
      {
        var garb = _lvlDescs[q].items[i];
        id.type = garb.id.type;
        for(int d = 0; d < 1<<garb.id.lvl; ++d)
        {
          id.lvl = 0;
          ids.Add(id);
        }
      }
    }
    vs.shuffle(1000);
    vs.Reverse();
    for(int q = 0; q < ids.Count; ++q)
    {
      var item = GameData.Prefabs.CreateItem(ids[q], _itemsContainer);
      if(vs.Count > 0)
      {
        item.Init(vs.first());
        vs.RemoveAt(0);
        item.Show();
        _grid.set(item.vgrid, 1);
        _items.Add(item);
      }
      else
      {
        item.Init(Vector2.zero);
        _items2.Add(item);
      }
    }
  }

  public void OnInputBeg(TouchInputData tid)
  {
    _itemSelected = null;
    _itemSelected = tid.GetClosestCollider(0.5f, Item.layerMask)?.GetComponent<Item>() ?? null;
    _itemSelected?.Select(true);
  }
  public void OnInputMov(TouchInputData tid)
  {
    if(Finished)
      return;
    if(_itemSelected)
    {
      _itemSelected.vwpos = tid.RaycastData.Value.point + new Vector3(0, 0.5f, 0);

      var _itemNearest = tid.GetClosestCollider(0.5f, Item.layerMask)?.GetComponent<Item>() ?? null;
      if(_itemNearest)
      {
        _itemNearest.Hover(true);
      }

    }
  }
  public void OnInputEnd(TouchInputData tid)
  {
    if(!_itemSelected)
      return;

    var itemHit = tid.GetClosestCollider(0.5f, Item.layerMask)?.GetComponent<Item>() ?? null;
    if(itemHit)
    {
      var newItem = Item.Merge(_itemSelected, itemHit, _items);
      if(newItem)
      {
        _grid.set(_itemSelected.vgrid, 0);
        newItem.Show();
        System.Array.ForEach(_lvlDescs, (lvlDesc) => lvlDesc.animal.AnimTalk());
      }
      else
      {
        _itemSelected.Select(false);
        _itemSelected.MoveBack();
      }
    }
    else
    {
      var animalHit = tid.GetClosestCollider(0.5f, Animal.layerMask)?.GetComponent<Animal>() ?? null;
      if(animalHit && animalHit.CanPut(_itemSelected))
      {
        animalHit.Put(_itemSelected);
        _grid.set(_itemSelected.vgrid, 0);
        _items.Remove(_itemSelected);
        CheckEnd();
      }
      else
      {
        _itemSelected.Select(false);
        _itemSelected.MoveBack();
      }
    }
    _itemSelected = null;
  }
  void Sequence()
  {
    // if(!_sequence)
    // {
    //   _sequence = true;
    //   StartCoroutine(coSequenece());
    // }
  }

  IEnumerator coEnd()
  {
    yield return new WaitForSeconds(1.0f);
    Succeed = true;
    onFinished?.Invoke(this);
    yield return new WaitForSeconds(0.5f);
    _uiSummary.Show(this);
  }
  void CheckEnd()
  {
    int activeAnimals = _animals.Count((animal) => animal.isActive);
    if(!Finished && activeAnimals == 0)
    {
      Finished = true;
      StartCoroutine(coEnd());
    }
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
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Vector3 vLB = new Vector3(-_dim.x * 0.5f, 0, -_dim.y * 0.5f);
    Vector3 vRT = new Vector3( _dim.x * 0.5f, 0, _dim.y * 0.5f);
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
