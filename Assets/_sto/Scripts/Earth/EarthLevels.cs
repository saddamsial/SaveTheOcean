using System;
using System.Linq;
using UnityEngine;

public class EarthLevels : MonoBehaviour
{
    [SerializeField] private UIButtonEarth _uiButtonEarth;
    [SerializeField] private LevelEarth[] _levelEarths;
    
    private LevelEarth _currentUnlockLevel;
    private LevelEarth _currentPassedLevel;
    private SelectLevel _selectLevel;

    public UIButtonEarth UIButtonEarth => _uiButtonEarth;
    public LevelEarth[] LevelEarths => _levelEarths;
    public SelectLevel SelectLevel => _selectLevel;
    
    void Awake()
    {
      Earth.onShow += Show;
    }
    void OnDestroy()
    {
      Earth.onShow -= Show;
    }


    void Show(int lvlIdx)
    {
      gameObject.SetActive(true);
      HandlerLevelEarthsState();
      SetFirstLevelUI();
    }
    private void HandlerLevelEarthsState()
    {
        var levels = GameState.Progress.Levels.GetStates();
        var size = _levelEarths.Length < levels.Length ? _levelEarths.Length : levels.Length; 
        for (var i = 0; i < size; i++)
        {
            switch (levels[i])
            {
                case Level.State.Locked:
                    _levelEarths[i].SetStateLevel(i,StateLevel.Lock);
                    break;
                case Level.State.Unlocked:
                    _levelEarths[i].SetStateLevel(i,StateLevel.Unlock);
                    break;
                case Level.State.Started:
                    break;
                case Level.State.Finished:
                    _levelEarths[i].SetStateLevel(i,StateLevel.Passed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }

    private void SetFirstLevelUI()
    {
        _currentUnlockLevel = _levelEarths.FirstOrDefault(level => level.StateLevel == StateLevel.Unlock);

        if (_currentUnlockLevel != null)
        {
            _selectLevel.SelectLevelEarth(_currentUnlockLevel);
            return;
        }
        
        _currentPassedLevel = _levelEarths.LastOrDefault(level => level.StateLevel == StateLevel.Passed);
        _selectLevel.SelectLevelEarth(_currentPassedLevel);
    }
    
    public void SetSelectLevel(SelectLevel selectLevel) => _selectLevel = selectLevel;
}