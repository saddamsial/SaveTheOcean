using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib;

public class Animal : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] Animator     _animator;
  [SerializeField] Transform    _garbageContainer;
  [SerializeField] FeedInfo     _feedingInfo;
  [SerializeField] GarbageInfo  _garbageInfo;

  [Header("Props")]
  [SerializeField] Type _type;
  [SerializeField] int  _baseLevelUp = 100;

  public enum Type
  {
    None,
    Dolphin,
    Turtle,
    Hammerfish,
    Octopus,
  }

  List<Item.ID>    _garbages = new List<Item.ID>();
  List<Item>       _garbagesCleared = new List<Item>();

  public Type          type => _type;
  public int           baseLevelUp => _baseLevelUp;
  public List<Item>    garbages {get; private set;} = new List<Item>();
  public bool          isActive  {get; private set;} = false;
  public bool          isReady  {get; private set;} = false;
  public int           requests => garbages.Count;
  public Vector3       garbagePos => _garbageContainer.transform.position;

  static public int layer = 0;
  static public int layerMask = 0;

  bool feedingMode = false;

  void Awake()
  {
    layer = gameObject.layer;
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(layer));
  }

  IEnumerator ShowInfo()
  {
    yield return _animator.WaitForAnimState("_active");
    isReady = true;
    if(!feedingMode)
      _garbageInfo.Show(garbages);
    else
      _feedingInfo.Show(this);  
  }
  public void Init(GameData.GarbCats[] garbCats)
  {
    _garbages = new List<Item.ID>();
    feedingMode = Level.mode == Level.Mode.Feeding;

    if(!feedingMode)
    {
      foreach(var gcat in garbCats)
      {
        var item = GameData.Prefabs.GetGarbagePrefab(gcat);
        _garbages.Add(item.id);
      }
      foreach(var id in _garbages)
      {
        garbages.Add(GameData.Prefabs.CreateStaticItem(id, _garbageInfo.itemContainer));
      }
    }
    else
    {
      
    }
  }
  public void Activate(bool show_info)
  { 
    isActive = true;
    _animator.SetTrigger("activate");
    GetComponent<Collider>().enabled = true;
    if(show_info)
      StartCoroutine(ShowInfo());
  }
  public void Deactivate()
  {
    isReady = false;
    isActive = false;
    _garbageInfo.Hide();
    _animator.SetTrigger("deactivate");
    GetComponent<Collider>().enabled = false;
    _animator.InvokeForAnimStateEnd("_deactivate", ()=> gameObject.SetActive(false));
  }
  public void AnimLvl()
  {
    isReady = false;
    isActive = false;
    _garbageInfo.Hide();
    _feedingInfo.Hide();
    _animator.SetTrigger("deactivate");
    GetComponent<Collider>().enabled = false;
    this.Invoke(() => this.Activate(true), 3.0f);
    //_animator.InvokeForAnimState("_inactive", ()=> Activate(true));
  }
  public void AnimThrow()
  {
    if(isReady)
      _animator.Play("itemPush");
  }
  public void AnimTalk()
  {
    if(isReady)
      _animator.Play("talk", 0);
  }
  public bool CanPut(Item item)
  {
    bool ret = false;
    if(isReady)
    {
      if(!feedingMode)
        ret = IsReq(item);
      else  
        ret = item.id.kind == Item.Kind.Food;
    }
    return ret;
  }
  public Item GetReq(Item item) => garbages.Find((garbage) => Item.EqType(item, garbage));
  public bool IsReq(Item item) => (!feedingMode)? GetReq(item) != null : item.id.kind == Item.Kind.Food;
  public void Feed(Item item)
  {
    bool next_lvl = GameState.Animals.Feed(type, item.id, _baseLevelUp);
    _feedingInfo.UpdateInfo();
    isReady = true;
    item.gameObject.SetActive(false);
    if(!next_lvl)
      AnimTalk();
    else
      AnimLvl();
  }
  public void Put(Item item)
  {
    //if(isReady)
    {
      Item it = garbages.Find((garbage) => Item.EqType(garbage, item));
      if(it)
      {
        _garbageInfo.Remove(it.id);
        _garbagesCleared.Add(it);
        garbages.Remove(it);
        item.gameObject.SetActive(false);
        GameObject model = null;
        if(isReady)
        {
          model = item.mdl;
          model.transform.parent = _garbageContainer;
          model.transform.localPosition = Vector2.zero;
          model.SetActive(true);
        }

        if(garbages.Count > 0)
        {
          if(isReady)
          {
            AnimThrow();
            StartCoroutine(_animator.InvokeForAnimStateEnd("itemPush", ()=> 
            {
              isReady = true;
              model.SetActive(false);
            }));
          }
          isReady = false;
        }
        else
        {
          isReady = false;
          Deactivate();
        }
      }
    }
  }
}
