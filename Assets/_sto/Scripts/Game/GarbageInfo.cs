using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshPro;
using GameLib;
using GameLib.Utilities;

public class GarbageInfo : MonoBehaviour
{
  [SerializeField] GameObject content;
  [SerializeField] GameObject requestContainer;
  [SerializeField] ActivatableObject _actObj;

  public static int layer { get; private set; } = 0;
  public static int layerMask { get; private set; } = 0;

  public static System.Action<GarbageInfo> onGoodItem, onWrongItem;
  public static System.Action<GarbageInfo> onShow, onHide;

  public Transform itemContainer => requestContainer.transform;
  List<Item> _requestedItems = new List<Item>();
  
  void Awake()
  {
  }
  public void Show(List<Item> requests)
  {
    _requestedItems.AddRange(requests);
    _requestedItems.ForEach((request) => request.transform.parent = requestContainer.transform);
    int beg = -_requestedItems.Count / 2;
    for(int q = 0; q < _requestedItems.Count; ++q)
    {
      _requestedItems[q].transform.localPosition = new Vector3((beg + q) * 0.5f, 0, 0);
    }
    _actObj.ActivateObject();
    onShow?.Invoke(this);
  }
  public void Remove(Item.ID id)
  {
    Item item = _requestedItems.Find((req) => Item.ID.Eq(req.id, id));
    if(item)
      item.gameObject.SetActive(false);
  } 
  public void Hide()
  {
    _actObj.DeactivateObject();
    onHide?.Invoke(this);
  }
}
