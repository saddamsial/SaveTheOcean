using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;

public class Earth : MonoBehaviour
{
  [SerializeField] GameObject _antonEarth;
  [SerializeField] Animator   _animator;

  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  ActivatableObject _actObj;

  void Awake()
  {
    _actObj = GetComponent<ActivatableObject>();
  }

  public void Show(int indexLevel)
  {
    _antonEarth.SetActive(true);
    _actObj.ActivateObject();
    onShow?.Invoke(indexLevel);
  }
  public void Hide()
  {
    _antonEarth.SetActive(true);
    _actObj.DeactivateObject();
    onHide?.Invoke();
    StartCoroutine(coHide());
  }
  IEnumerator coHide()
  {
    yield return null;
    while(_actObj.InTransition)
      yield return null;
    
    _antonEarth.SetActive(false);
  }
}
