using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthClazz : MonoBehaviour
{
  public static System.Action<int> onLevelStart;

  [SerializeField] GameObject _antonEarth;

  public void Show(int levelIdx, Level.State[] levels_states)
  {
    //init levels' state on earth
    //....

    //show earth
    _antonEarth.SetActive(true);
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.A))
    {
      int selected_level_idx = 0;
      onLevelStart?.Invoke(selected_level_idx);
      _antonEarth.SetActive(false);
    }
  }
}
