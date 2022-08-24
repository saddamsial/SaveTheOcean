using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplitMachine : MonoBehaviour
{
  [SerializeField] Collider dropCollider;
  [SerializeField] Collider splitCollider0;
  [SerializeField] Collider splitCollider1;

  [SerializeField] Item    _dropSlot = null;
  [SerializeField] Item[]  _splitSlots = new Item[2]{null, null};

  public bool IsDropSlot(Collider coll) => coll == dropCollider;
  
  public bool AreSplitSlotsEmpty => Array.TrueForAll(_splitSlots, (slot) => slot == null);
  public Item dropSlot => _dropSlot;
  public Item splitSplits(int idx) => _splitSlots[idx];
}
