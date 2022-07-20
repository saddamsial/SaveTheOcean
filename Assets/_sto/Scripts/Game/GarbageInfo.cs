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
  Item  _requestedItem = null;
  
  void Awake()
  {
  }
  public void Show(Item request)
  {
    _requestedItem = request;
    _requestedItem.transform.parent = requestContainer.transform;
    //System.Array.ForEach(_requestedItem.GetComponentsInChildren<Transform>(), (Transform tr) =>  tr.ResetTransform());
    _actObj.ActivateObject();
    onShow?.Invoke(this);
  }
  public void Hide()
  {
    _actObj.DeactivateObject();
    onHide?.Invoke(this);
  }
}
