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
  // IEnumerator WaitForAnimState(string anim, System.Action action = null)
  // {
  //   yield return null;
  //   while(!_animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
  //   {
  //     yield return null;
  //   }
  //   yield return null;

  //   action?.Invoke();
  // }
  // IEnumerator WaitForAnimStateEnd(string anim, System.Action<T> action = null)
  // {
  //   yield return null;
  //   while(_animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
  //   {
  //     yield return null;
  //   }
  //   yield return null;

  //   action?.Invoke<T>(T);    
  // }
  public void Init(Item[] items_prefab)
  {
    _garbages = new List<Item.ID>();
    foreach(var item in items_prefab)
    {
      _garbages.Add(item.id);
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
  public bool CanPut(Item item) => isReady && garbages.Find((garbage)=> Item.EqType(item, garbage)) != null;
  public void Put(Item item)
  {
    if(isReady)
    {
      Item it = garbages.Find((garbage) => Item.EqType(garbage, item));
      if(it)
      {
        var model = item.mdl;
        model.transform.parent =  _garbageContainer;
        model.transform.localPosition = Vector2.zero;

        _garbageInfo.Remove(it.id);
        _garbagesCleared.Add(it);
        garbages.Remove(it);
        item.gameObject.SetActive(false);
        model.SetActive(true);
        if(garbages.Count > 0)
        {
          AnimThrow();
          isReady = false;
          StartCoroutine(_animator.InvokeForAnimStateEnd("itemPush", ()=> 
          {
            isReady = true;
            model.SetActive(false);
          })
          );
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
