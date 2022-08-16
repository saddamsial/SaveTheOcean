using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;

public class Earth : MonoBehaviour
{
  [SerializeField] GameObject _antonEarth;
  //[SerializeField] Animator   _animator;

  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  //ActivatableObject _actObj;

  void Awake()
  {
    //_actObj = GetComponent<ActivatableObject>();
  }

  public void Show(int indexLevel)
  {
    _antonEarth.SetActive(true);
    onShow?.Invoke(indexLevel);    
  }
  public void Hide()
  {
    onHide?.Invoke();
    _antonEarth.SetActive(false);
  }
}
