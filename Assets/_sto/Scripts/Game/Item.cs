using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;
using GameLib.Utilities;

public class Item : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] GameObject         _modelContainer;
  [SerializeField] ActivatableObject  _activatable;
  [SerializeField] SpringMove         _sm;
  [Header("Settings")]
  [SerializeField] float _moveStep = 1.0f;

  List<GameObject> _models = new List<GameObject>();

  public struct ID
  {
    [SerializeField] int _type;
    [SerializeField] int _lvl;

    public ID(int item_type, int item_lvl)
    {
      _type = item_type;
      _lvl = item_lvl;
    }

    public int type {get => _type; set{_type = value;}}
    public int lvl {get => _lvl; set{_lvl = value;}}
    public static bool Eq(ID id0, ID id1) => id0.type == id1.type && id0.lvl == id1.lvl;
  }

  ID         _id = new ID();
  float      _lifetime = 0;
  Vector2    _grid = Vector2.zero;
  Vector2Int _agrid = Vector2Int.zero;
  Vector3[]  _path = new Vector3[4];
  Vector3?   _vdstPos = null;
  Vector3    _vdim = Vector3.one;
  Vector3    _vbtmExtent = Vector3.zero;
  Vector3    _vmin = Vector3.zero;
  Vector3    _vmax = Vector3.zero;

  public static float gridSpace = 1.0f;
  public static System.Action<Item> onShow, onShown, onMerged, onPut, onHide;
  public static Item Merge(Item item0, Item item1, List<Item> _items)
  {
    if(EqType(item0, item1) && item1.IsUpgradable)
    {
      item0.Hide();
      _items.Remove(item0);
      onMerged?.Invoke(item1);
      return Upgrade(item1, _items);
    }
    return null;
  }
  public static Item Upgrade(Item item, List<Item> _items)
  {
    Item new_item = null;
    if(item.IsUpgradable)
    {
      item.incLvl();
      new_item = GameData.Prefabs.CreateItem(item.id, item.transform.parent);
      item.Hide();
      new_item.Init(item.vgrid);

      _items.Remove(item);
      _items.Add(new_item);
    }
    return new_item;
  }
  public static Vector3 ToPos(Vector2 vgrid) => new Vector3(vgrid.x, 0, vgrid.y) * Item.gridSpace;
  public static bool    EqType(Item item0, Item item1)
  {
    return item0 != null && item1 != null && ID.Eq(item0.id, item1.id);
  }
  //public static Vector3 itemsOffset = new Vector3(0,0,0);

  static public int layer = 0;
  static public int layerMask = 0;

  public ID         id { get => _id; set { _id = value; } }
  public GameObject mdl => _models[0];
  public Vector3    vdim => _vdim;
  public Vector3    vbtmExtent => _vbtmExtent;
  public Vector2    vgrid {get => _grid; set{_grid = value;}}
  public Vector2Int agrid {get => _agrid; set{_agrid = value;}}
  public Vector3    vlpos {get => transform.localPosition; set{transform.localPosition = value;}}
  public Vector3    vwpos {  get => transform.position; set { transform.position = value;}}
  public Vector3?   vdstPos { get=> _vdstPos; set { _vdstPos = value;}}
  public Vector3    gridPos => Item.ToPos(vgrid);
  public bool       IsMaxLevel => id.lvl + 1 == GameData.Prefabs.ItemLevelsCnt(id.type);
  public bool       IsUpgradable => id.lvl + 1 < GameData.Prefabs.ItemLevelsCnt(id.type);
  public bool       IsSelected {get; set;}
  public void       incLvl(){_id.lvl++;}
  

  void Awake()
  {
    //_mrs = _modelContainer.GetComponentsInChildren<MeshRenderer>();
    layer = gameObject.layer;
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(layer));

    for(int q = 0; q < _modelContainer.transform.childCount; ++q)
      _models.Add(_modelContainer.transform.GetChild(q).gameObject);
  }
  public void SetAsStatic()
  {
    _activatable.ActivateObject();
    GetComponent<BoxCollider>().enabled = false;
    System.Array.ForEach(GetComponentsInChildren<ObjectRandomizeTransform>(), (ort) => ort.transform.reset());
    SetModel(0);
  }
  public void Init(Vector2 grid)
  {
    vgrid = grid;
    vlpos = Item.ToPos(vgrid);
    GetComponent<BoxCollider>().enabled = false;
    SetModel(0);//_models.get_random_idx());
  }
  void SetModel(int model_idx)
  {
    for(int q = 0; q < _models.Count; ++q)
      _models[q].SetActive(q == model_idx);

    _vdim = Vector3.zero;
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    System.Array.ForEach(renderers, (rend) => 
    {
      _vdim = Vector3.Max(_vdim, rend.bounds.extents);
      _vmin = Vector3.Min(_vmin, rend.bounds.min);
      _vmax = Vector3.Min(_vmax, rend.bounds.max);
    });
    _vbtmExtent.y = -_vmin.y;// * _models[model_idx].transform.localScale.y;
    //_vbtmExtent.y = _vdim.y;
    _vdim *= 2.0f;
  }
  public bool IsReady => !_activatable.InTransition && _lifetime > 0.125f;
  public void Show()
  {
    _activatable.ActivateObject();
    this.Invoke(()=> GetComponent<Collider>().enabled = true, 0.5f);
    onShow?.Invoke(this);
  }
  public void Spawn(Vector2 vgrid, Vector3[] vpath, float speed = 1)
  {
    Init(vgrid);
    gameObject.SetActive(true);
    GetComponent<Collider>().enabled = false;
    _activatable.ActivateObject();
    if(vpath != null)
    {
      System.Array.Copy(vpath, _path, 3);
      _path[3] = ToPos(vgrid);
      //_path[3] += itemsOffset;      
    }
    else
    {
      float immers = -6;
      _path[0] = ToPos(vgrid);
      _path[0].y = immers;
      _path[3] = ToPos(vgrid);
      //_path[3] += itemsOffset;

      _path[1] = Vector3.Lerp(_path[0], _path[3], 0.25f);
      _path[2] = Vector3.Lerp(_path[0], _path[3], 0.75f);
    }

    vwpos = _path[0];
    onShow?.Invoke(this);
    StartCoroutine(MovePath(speed));
  }
  IEnumerator MovePath(float speed)
  {
    float t = 0.0f;
    while(t <= 1)
    {
      float prev_t = t;
      t += Time.deltaTime * speed;
      float tc = Mathf.Clamp01(t);
      vwpos = Vector3Ex.bezier(_path, tc);
      if(prev_t < 0.8f && t >= 0.8f)
        onShown?.Invoke(this);
      yield return null;
    }
    
    _sm.Touch(15f);
    _path = null;
    GetComponent<Collider>().enabled = true;
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
    // if(IsSelected)
    //   vwpos = Vector3.MoveTowards(vwpos, vdstPos.Value, _moveStep * Time.deltaTime);
  }
  // void OnDrawGizmos()
  // {
  //   Gizmos.DrawCube(transform.position + new Vector3(0,1.0f, 0), Vector3.one * 0.25f);
  // }
}
