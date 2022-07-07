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
  [SerializeField] Transform _poiLT;
  [SerializeField] Transform _poiRB;

  [Header("Settings")]
  [SerializeField] Vector2Int _dim;
  [SerializeField] float      _gridSpace = 1.0f;
  [Header("Items")]
  [SerializeField] List<Item> _listItems;

  public class Grid
  {
    Vector2Int  _dim = Vector2Int.zero;
    Item[,]     _grid = null;
    GridElem[,] _elems = null;

    public void Init(Vector2Int dims)
    {
      _grid = new Item[dims.y, dims.x];
      _elems = new GridElem[dims.y, dims.x];
      _dim = dims;
    }

    public void Clear()
    {
      System.Array.Clear(_grid, 0, _grid.Length);
    }
    public Vector2Int dim => _dim;
    public bool IsInside(Vector2Int v)
    {
      return v.x >= -_dim.x / 2 && v.x <= _dim.x / 2 && v.y >= -_dim.y / 2 && v.y <= _dim.y / 2;
    }

    public void Set(Item item)
    {
      Set(item.vgrid, item);
    }
    public void Set(Vector2Int igrid, Item item)
    {
      var v = igrid + _dim / 2;
      _grid[v.y, v.x] = item;
    }
    public Item Get(Vector2Int grid)
    {
      return GetA(grid + _dim/2);
    }
    public Item GetA(Vector2Int grid)
    {
      Item item = null;
      if(grid.x >= 0 && grid.x < dim.x && grid.y >= 0 && grid.y < dim.y)
        item = _grid[grid.y, grid.x];
      
      return item;
    }
    public Vector2Int Clamp(Vector2Int igrid)
    {
      return new Vector2Int(Mathf.Clamp(igrid.x, -_dim.x/2, _dim.x/2), Mathf.Clamp(igrid.y, -_dim.y/2, _dim.y/2));
    }
    // public List<Item> GetNB(Vector2Int v)
    // {
    //   List<Item> list = new List<Item>();
    //   Vector2Int vv = Vector2Int.zero;
    //   for(int y = -1; y <= 1; ++y)
    //   {
    //     vv.y = y;
    //     for(int x = -1; x <= 1; ++x)
    //     {
    //       vv.x = x;
    //       if(x != 0 || y != 0)
    //       {
    //         var item = Get(v + vv);
    //         if(item != null)
    //           list.Add(item);
    //       }
    //     }
    //   }
    //   return list;
    // }
    // public void update(List<Item> _items, Level lvl)
    // {
    //   Clear();
    //   for(int q = 0; q < _items.Count; ++q)
    //   {
    //     if(IsInside(_items[q].grid))
    //       Set(_items[q]);
    //   }
    // }
    public GridElem GetElem(Vector2Int vi)
    {
      if(IsInside(vi))
      {
        var v = _dim / 2 + vi;
        return _elems[v.y, v.x];      
      }
      else
        return null;  
    }
    public void SetElem(GridElem elem)
    {
      var v = _dim / 2 + elem.vgrid;
      _elems[v.y, v.x] = elem;
    }    
    public GridElem[,] elems => _elems;
  }

  public int    LevelIdx => GameState.Progress.Level;
  public bool   Succeed {get; private set;}
  public bool   Finished {get; private set;}
  public int    Points {get; set;} = 0;
  public int    Stars {get; set;}
  public Vector2Int Dim => _dim;

  bool      _started = false;
  UISummary _uiSummary = null;

  Item       _itemSelected;
  List<Item> _items = new List<Item>();
  Transform  _cameraContainer = null;
  bool       _inputBlocked = false;
  bool       _sequence = false;
  Grid       _grid = new Grid();


  void Awake()
  {
    Item.GridSpace = _gridSpace;
    // if(_usePerLevelPOI)
    // {
    //   _poiLT.position = new Vector3(-_grid.dim().x/2 - 2, 0, _grid.dim().y / 2 + 2);
    //   _poiRB.position = new Vector3(_grid.dim().x / 2 + 2, 0, -_grid.dim().y / 2 - 2);
    // }
    _cameraContainer = GameObject.Find("_cameraContainer").transform;
    _items = _itemsContainer.GetComponentsInChildren<Item>().ToList();
    _uiSummary = FindObjectOfType<UISummary>(true);
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
    _started = true;
    onStart?.Invoke(this);
  }
  void Init()
  {
    _grid.Init(_dim);
    for(int y = 0; y < _dim.y; ++y)
    {
      float yy = Mathf.RoundToInt((-_dim.y + 1) * 0.5f + y);
      for(int x = 0; x < _dim.x; ++x)
      {
        float xx = Mathf.RoundToInt((-_dim.x + 1) * 0.5f + x);
        // var ge = GameData.Prefabs.CreateGridElem(_gridContainer);
        // ge.transform.localPosition = new Vector3(xx * scale, 0.05f, yy * scale);
        // ge.vgrid = new Vector2Int((int)xx, (int)yy);
        // ge.Show();
        // _grid.SetElem(ge);
        var item = GameData.Prefabs.CreateItem(-1, 0, _itemsContainer);
        item.Init(new Vector2Int((int)xx, (int)yy));
        item.Show();
        _items.Add(item);
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
        newItem.Show();
      else
      {
        _itemSelected.Select(false);
        _itemSelected.MoveBack();
      }  
    }
    else
    {
      _itemSelected.Select(false);
      _itemSelected.MoveBack();
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
    if(!Finished)
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
