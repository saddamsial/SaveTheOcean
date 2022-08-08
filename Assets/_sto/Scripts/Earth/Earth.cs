using System;
using UnityEngine;

[RequireComponent(typeof(EarthLevels))]
public class Earth : MonoBehaviour
{

  public static System.Action<int> onLevelStart;
  
    private EarthLevels _earthLevels;

    private void Start()
    {
        _earthLevels = GetComponent<EarthLevels>();
    }

    public void Show(int indexLevel)
    {
  /*      if (indexLevel < _earthLevels.LevelEarths.Length || indexLevel > _earthLevels.LevelEarths.Length)
        {
            Debug.LogError("Error index level");
            return;
        }*/
        
   //     _earthLevels.SelectLevel.SelectLevelEarth(_earthLevels.LevelEarths[indexLevel]);
    }
  private void OnEnable()
  {
    onLevelStart += Disable;
    UIButtonEarth.OnEnableButton?.Invoke();
  }

  private void OnDisable()
  {
    onLevelStart -= Disable;
  }

  private void Disable(int i) => gameObject.SetActive(false); 
}
