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
  [SerializeField] GarbageInfo  _garbageInfo;


  List<Item.ID>    _garbages = new List<Item.ID>();
  List<Item>       _garbagesCleared = new List<Item>();

  public List<Item>    garbages {get; private set;} = new List<Item>();
  public bool          isActive  {get; private set;} = false;
  public bool          isReady  {get; private set;} = false;
  public int           requests => garbages.Count;
  public Vector3       garbagePos => _garbageContainer.transform.position;

  static public int layer = 0;
  static public int layerMask = 0;

  void Awake()
  {
    layer = gameObject.layer;
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(layer));
  }

  IEnumerator ShowGarbageInfo()
  {
    //yield return StartCoroutine(WaitForAnimState("_active"));
    yield return _animator.WaitForAnimState("_active");
    isReady = true;
    _garbageInfo.Show(garbages);
  }
  public void Init(GameData.GarbCats[] garbCats)
  {
    _garbages = new List<Item.ID>();
    bool isFeedingMode = Level.mode == Level.Mode.Feeding;
    foreach(var gcat in garbCats)
    {
      var item = GameData.Prefabs.GetGarbagePrefab(gcat);
      if(!isFeedingMode)
        _garbages.Add(item.id);
      else
        _garbages.Add(GameData.Prefabs.GarbToFood(gcat));
    }
    foreach(var id in _garbages)
    {
      garbages.Add(GameData.Prefabs.CreateStaticItem(id, _garbageInfo.itemContainer));
    }
  }
  public void Activate(bool show_garbage_info)
  { 
    isActive = true;
    _animator.SetTrigger("activate"); 
    if(show_garbage_info)
      StartCoroutine(ShowGarbageInfo());
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
  public bool CanPut(Item item) => isReady && IsReq(item);
  public bool IsReq(Item item) => garbages.Find((garbage) => Item.EqType(item, garbage)) != null;
  public void Put(Item item, bool feedingMode)
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
        if(!feedingMode)
        {
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
        else
        {
          var model = item.mdl;
          model.SetActive(false);
          if(garbages.Count > 0)
          {
            isReady = true;
            AnimTalk();
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
}
