using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib;

public class FeedInfo : MonoBehaviour
{
  [SerializeField] ActivatableObject _actObj;
  [SerializeField] Slider _slider;
  [SerializeField] TMPLbl _lblLvl;

  [Header("misc")]
  [SerializeField] string _strLvlFmt = "lvl {0}";  

  public static System.Action<FeedInfo> onShow, onHide;

  Animal.Type _animalType = Animal.Type.None;
  int         _animalLevelUp = 0;

  float _destVal = 0;
  float _destBeg = 0;
  float _destEnd = 0;
  int   _destLvl = 0;

  void lvlText(int lvl) => _lblLvl.text = string.Format(_strLvlFmt, lvl + 1);

  void Awake()
  {
    //SetupInfo();   
  }

  void SetupInfo()
  {
    var info = GameState.Animals.GetInfo(_animalType, _animalLevelUp);
    _destVal = _slider.value = info.kcal;
    _destBeg = _slider.minValue = info.lvlRng.beg;
    _destEnd = _slider.maxValue = info.lvlRng.end;
    _destLvl = info.lvl;
    lvlText(_destLvl);
  }
  public void UpdateInfo()
  {
    var info = GameState.Animals.GetInfo(_animalType, _animalLevelUp);
    _destVal = info.kcal;
    _destBeg = info.lvlRng.beg;
    _destEnd = info.lvlRng.end;
    _destLvl = info.lvl;
  }
  public void Show(Animal animal)
  {
    _animalType = animal.type;
    _animalLevelUp = animal.baseLevelUp;
    SetupInfo();
    _actObj.ActivateObject();
    onShow?.Invoke(this);
  }
  public void Hide()
  {
    _actObj.DeactivateObject();
    onHide?.Invoke(this);
  }
  void Update()
  {
    _slider.value = Mathf.Lerp(_slider.value, _destVal, Time.deltaTime * 4);
    if(_slider.value >= _slider.maxValue - 1)
    {
      _slider.minValue = _destBeg;
      _slider.maxValue = _destEnd;
      lvlText(_destLvl);
    }
  }
}
