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
  [SerializeField] Item      _garbagePrefab; 

  static public int layer = 0;
  static public int layerMask = 0;

  public Item garbage {get; private set;}

  void Awake()
  {
    layer = gameObject.layer;
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(layer));
    garbage = GameData.Prefabs.CreateStaticItem(_garbagePrefab, _garbageInfo.itemContainer);
  }

  IEnumerator ShowGarbageInfo()
  {
    yield return StartCoroutine(WaitForAnimState("_active"));
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

  public void Activate(bool show_garbage_info)
  { 
    _animator.SetTrigger("activate"); 
    if(show_garbage_info)
      StartCoroutine(ShowGarbageInfo());
  }
  public void Deactivate()
  {
    _garbageInfo.Hide();
    GetComponent<Collider>().enabled = false;
    this.Invoke(() => _animator.SetTrigger("deactivate"), 0.5f);
    this.Invoke(() => gameObject.SetActive(false), 4.0f);
  }
  public void AnimFailed() => _animator.SetTrigger("fail");
  public void AnimTalk() => _animator.Play("talk",0);
  public bool CanPut(Item item) => Item.EqType(item, garbage);
  public void Put(Item item)
  {
    item.transform.parent = _garbageContainer;
    item.transform.reset();
    this.Invoke(()=> item.gameObject.SetActive(false), 2.0f);
  }
  // void Update()
  // {
  //   //_uiTransform.position = _armature.position;
  //   //_uiTransform.rotation = _armature.rotation;
  // }
}
