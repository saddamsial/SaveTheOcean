using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.Utilities;

public class Item : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] MeshRenderer[]     _mrs = null;
  [SerializeField] GameObject         _modelContainer;
  [SerializeField] ActivatableObject  _activatable;
  [SerializeField] ObjectColorBlender _ocb;
  [SerializeField] ObjectShake        _objShake;
  [SerializeField] SpringMove         _sm;

  [Header("Props")]
  [SerializeField] Color _color;

  MaterialPropertyBlock _mpb = null;
  float                 _lifetime = 0;
  Vector2Int            _grid = Vector2Int.zero;
  
  public int type {get;set;} = 0;
  public int lvl  {get;set;} = 0;


  public static float GridSpace = 1.0f;

  public static System.Action<Item> onShow, onHide, onMerged;
  public static Item Merge(Item item0, Item item1, List<Item> _items)
  {
    if(EqType(item0, item1) && item1.IsUpgradable)
    {
      item0.Hide();
      _items.Remove(item0);
      return Upgrade(item1, _items);
    }
    return null;
  }
  public static Item Upgrade(Item item, List<Item> _items)
  {
    Item new_item = null;
    if(item.IsUpgradable)
    {
      new_item = GameData.Prefabs.CreateItem(item.type, item.lvl + 1, item.transform.parent);
      item.Hide();
      new_item.Init(item.vgrid);

      _items.Remove(item);
      _items.Add(new_item);
    }
    return new_item;
  }
  static Vector3     ToPos(Vector2Int vgrid) => new Vector3(vgrid.x, 0, vgrid.y) * Item.GridSpace;
  public static bool EqType(Item item0, Item item1)
  {
    return item0 != null && item1 != null && item0.lvl == item1.lvl && item0.type == item1.type;
  }

  static public int layer = 0;
  static public int layerMask = 0;

  public Vector2Int vgrid {get => _grid; set{_grid = value;}}
  public Vector3    vlpos {get => transform.localPosition; set{transform.localPosition = value;}}
  public Vector3    vwpos { get => transform.position; set { transform.position = value;}}
  // public Color      color
  // {
  //   get => _color;
  //   set
  //   {
  //     _color = value;
  //     //_mr.material.color = _color;
  //     foreach(var _mr in _mrs)
  //     {
  //       _mpb.SetColor("_MainColor", _color);
  //       _mpb.SetColor("_BaseColor", _color);
  //       _mr.SetPropertyBlock(_mpb);
  //     }
  //   }
  // }
  public bool IsUpgradable => lvl + 1 < GameData.Prefabs.ItemLevelsCnt(type);
  public bool IsSelected {get; set;}

  void Awake()
  {
    _mrs = _modelContainer.GetComponentsInChildren<MeshRenderer>();
    layer = gameObject.layer;
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(layer));
  }
  public void Init(Vector2Int grid)
  {
    _mpb = new MaterialPropertyBlock();
    _mrs[0]?.GetPropertyBlock(_mpb, 0);
    //color = _color;
    vgrid = grid;
    vlpos = Item.ToPos(vgrid);
  }
  public bool IsReady => !_activatable.InTransition && _lifetime > 0.125f;
  public void Show()
  {
    _activatable.ActivateObject();
    onShow?.Invoke(this);
  }
  public void Hide(bool silent = false)
  {
    gameObject.SetActive(false);
    onHide?.Invoke(this);
    // if(!silent)
    //   onHide?.Invoke(this);
    // StartCoroutine(WaitForEnd());
  }
  public void Deactivate()
  {
    _activatable.DeactivateObject();
  }
  IEnumerator WaitForEnd()
  {
    yield return null;
    while(_activatable.InTransition)
    {
      yield return null;
    }
    gameObject.SetActive(false);
  }
  public void Select(bool sel)
  {
    var coll = GetComponent<Collider>();
    if(coll)
      coll.enabled = !sel;
    IsSelected = sel;  
  }
  public void MoveBack()
  {
    vlpos = Item.ToPos(vgrid);
  }
  public void Hover(bool act)
  {
    if(act)
      _sm.Touch(-0.25f);
  }

  void Update()
  {
    _lifetime += Time.deltaTime;
  }
  void OnDrawGizmos()
  {
    Gizmos.color = _color;
    Gizmos.DrawCube(transform.position + new Vector3(0,1.0f, 0), Vector3.one * 0.25f);
  }
}
