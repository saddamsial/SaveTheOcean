using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;

public class Animal : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] Animator  _animator;
  [SerializeField] Transform _garbageContainer;
  [SerializeField] GarbageInfo _garbageInfo;

  static public int layer = 0;
  static public int layerMask = 0;

  public Item garbage {get; private set;} = null;
  public bool isActive  {get; private set;} = false;
  public bool isReady { get; private set; } = false;

  void Awake()
  {
    layer = gameObject.layer;
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(layer));
  }

  IEnumerator ShowGarbageInfo()
  {
    yield return StartCoroutine(WaitForAnimState("_active"));
    isReady = true;
    _garbageInfo.Show(garbage);
  }
  IEnumerator WaitForAnimState(string anim)
  {
    yield return null;
    while(!_animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
    {
      yield return null;
    }
    yield return null;
  }
  public void Init(Item item_prefab)
  {
    garbage = GameData.Prefabs.CreateStaticItem(item_prefab.id, _garbageInfo.itemContainer);
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
    this.Invoke(() => gameObject.SetActive(false), 4.0f);
  }
  public void AnimFailed() => _animator.SetTrigger("fail");
  public void AnimTalk()
  {
    if(isReady)
      _animator.Play("talk", 0);
  }
  public bool CanPut(Item item) => Item.EqType(item, garbage) && isReady;
  public void Put(Item item)
  {
    if(isReady)
    {
      isReady = false;
      item.transform.parent = _garbageContainer;
      item.transform.reset();
      this.Invoke(()=> item.gameObject.SetActive(false), 2.0f);
    }
  }
}
