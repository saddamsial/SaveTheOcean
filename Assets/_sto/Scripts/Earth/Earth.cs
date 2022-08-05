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

    public void Show(int indexLevel, Level.State[] states)
    {
  /*      if (indexLevel < _earthLevels.LevelEarths.Length || indexLevel > _earthLevels.LevelEarths.Length)
        {
            Debug.LogError("Error index level");
            return;
        }*/
        
        _earthLevels.SelectLevel.SelectLevelEarth(_earthLevels.LevelEarths[0]);
      
      //fire selected level
      //int selected_level = 1;
      //onLevelStart?.Invoke(selected_level);
    }

  #if UNITY_EDITOR
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.A))
      {
        int selected_level = 1;
        onLevelStart?.Invoke(selected_level);
        //hide earth
        gameObject.SetActive(false);
      }
    }
  #endif

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
