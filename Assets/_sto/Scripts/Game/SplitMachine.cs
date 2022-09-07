using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplitMachine : MonoBehaviour
{
  [SerializeField] Collider    _dropCollider;
  [SerializeField] Transform[] _splitsContainers;

  [SerializeField] List<Item> _dropSlots = new List<Item>();
  [SerializeField] Item[]     _splitSlots = new Item[2]{null, null};
  

  List<Item> _itemsRef = null;

  public enum DropResult
  {
    Ok,
    NoCapacity,
    NoSplittableItem,
  }
  DropResult _dropResult = DropResult.Ok;

  public static int layerMask = -1;

  void Awake()
  {
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(_dropCollider.gameObject.layer));
  }

  public static Action<SplitMachine> onDropped, onSplitted;
  public  void Init(List<Item> _items) =>_itemsRef = _items;
  public  bool IsDropSlot(Collider coll) => coll == _dropCollider;
  private bool IsSplitsEmpty => Array.TrueForAll(_splitSlots, (slot) => slot == null);
  public  int  capacity => GameState.SplitMachine.capacity;
  public  bool IsReady => _dropSlots.Count < capacity;

  public  DropResult dropResult => _dropResult;
  public  Vector3 dropPosition => _dropCollider.transform.position;
  public  void    DropDone() => _dropResult = DropResult.Ok;
  public  void    DropNoCapacity() => _dropResult = DropResult.NoCapacity;
  public  void    DropNoSplittable() => _dropResult = DropResult.NoSplittableItem;

  public  void AddToDropSlot(Item item)
  {
    _dropSlots.Add(item);
    item.IsInMachine = true;
    onDropped?.Invoke(this);
    if(IsSplitsEmpty)
      Split();
    else
      item.vwpos = _dropCollider.transform.position;
  }
  private void    Split()
  {
    if(_dropSlots.Count > 0)
    {
      var new_items = Item.Split(_dropSlots[0], _itemsRef);
      if(new_items != null)
      {
        AddToSplitSlots(new_items);
        _dropSlots[0].Hide();
        _dropSlots.RemoveAt(0);
      }
    }
  }
  public void     RemoveFromSplitSlot(Item _item)
  {
    for(int q = 0; q < _splitSlots.Length; ++q)
    {
      if(_splitSlots[q] == _item)
      {
        _item.IsInMachine = false;
        _splitSlots[q] = null;
      }
    }
    if(IsSplitsEmpty)
      Split();
  }
  public Vector3? GetSplitSlotPos(Item item)
  {
    Vector3? vpos = null;
    for(int q = 0; q < _splitSlots.Length; ++q)
    {
      if(_splitSlots[q] == item)
        vpos = _splitsContainers[q].position;
    }

    return vpos;
  }
  public Vector3  GetSplitSlotPos(int idx) => _splitsContainers[idx].position;
  public void     AddToSplitSlots(Item[] items)
  {
    for(int q=0; q < items.Length; ++q)
    {
      _splitSlots[q] = items[q];
      _splitSlots[q].transform.position = _splitsContainers[q].position;
      _splitSlots[q].Show();
    }
    onSplitted?.Invoke(this);
  }
}
