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
  //[SerializeField] Transform _poiLT;
  //[SerializeField] Transform _poiRB;

  [Header("Settings")]
  [SerializeField] Vector2Int _dim;
  [SerializeField] float      _gridSpace = 1.0f;
  [Header("Items")]
  [SerializeField] List<Item> _listItems;

  public int    LevelIdx => GameState.Progress.Level;
  public bool   Succeed {get; private set;}
  public bool   Finished {get; private set;}
  public int    Points {get; set;} = 0;
  public int    Stars {get; set;}
  public Vector2Int Dim => _dim;

  bool      _started = false;
  UISummary _uiSummary = null;
  Transform _cameraContainer = null;
  Animal[]  _animals;

  Item       _itemSelected;
  List<Item> _items = new List<Item>();

  void Awake()
  {
    Item.GridSpace = _gridSpace;
    _cameraContainer = GameObject.Find("_cameraContainer").transform;
    _items = _itemsContainer.GetComponentsInChildren<Item>().ToList();
    _uiSummary = FindObjectOfType<UISummary>(true);
    _animals = _animalsContainer.GetComponentsInChildren<Animal>(true);

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
    
    System.Array.ForEach(_animals, (animal) => animal.Activate());
    
    onStart?.Invoke(this);
  }
  void Init()
  {
    for(int y = 0; y < _dim.y; ++y)
    {
      float yy = (-_dim.y + 1) * 0.5f + y;
      for(int x = 0; x < _dim.x; ++x)
      {
        float xx = (-_dim.x + 1) * 0.5f + x;
        var item = GameData.Prefabs.CreateItem(-1, 0, _itemsContainer);
        item.Init(new Vector2(xx, yy));
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
