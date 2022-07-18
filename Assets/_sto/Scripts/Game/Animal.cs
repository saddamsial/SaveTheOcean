using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
  [SerializeField] Animator  _animator;
  [SerializeField] Transform _itemContainer;

  public void Activate()
  {
    _animator.SetTrigger("activate");
  }
}
