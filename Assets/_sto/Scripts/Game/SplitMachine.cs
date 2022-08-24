using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplitMachine : MonoBehaviour
{
  [SerializeField] Collider    _dropCollider;
  [SerializeField] Transform[] _splitsContainers;

  [SerializeField] Item    _dropSlot = null;
  [SerializeField] Item[]  _splitSlots = new Item[2]{null, null};

  public bool IsDropSlot(Collider coll) => coll == _dropCollider;
  
  // public void Split(Item item, List<Item> list_items)
  // {
    
  // }
  public bool IsReady => dropSlot == null && AreSplitSlotsEmpty;
  public bool AreSplitSlotsEmpty => Array.TrueForAll(_splitSlots, (slot) => slot == null);
  public Item dropSlot => _dropSlot;
  public void Remove(Item _item)
  {
    for(int q = 0; q < _splitSlots.Length; ++q)
    {
      if(_splitSlots[q] == _item)
        _splitSlots[q] = null;
    }
  }
  public Item splitSlot(int idx) => _splitSlots[idx];
  public void addSplited(Item[] items)
  {
    for(int q=0; q < items.Length; ++q)
    {
      _splitSlots[q] = items[q];
      _splitSlots[q].transform.SetParent(_splitsContainers[q]);
      _splitSlots[q].transform.localPosition = Vector3.zero;
      _splitSlots[q].Show();
    }
  }
}
