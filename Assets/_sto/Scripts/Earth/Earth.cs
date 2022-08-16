using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib;

public class Earth : MonoBehaviour
{
  [SerializeField] GameObject _earthPrefab;

  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  public void Show(int indexLevel)
  {
    _earthPrefab.SetActive(true);
    onShow?.Invoke(indexLevel);    
  }
  public void Hide()
  {
    onHide?.Invoke();
    _earthPrefab.SetActive(false);
  }
}
