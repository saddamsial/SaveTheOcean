using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitMachine : MonoBehaviour
{
  [SerializeField] Collider dropCollider;
  [SerializeField] Collider splitCollider0;
  [SerializeField] Collider splitCollider1;


  public bool IsDropSlot(Collider coll) => coll == dropCollider;

  
}
