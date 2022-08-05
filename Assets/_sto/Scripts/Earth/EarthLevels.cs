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
    
    private void Start()
    {
        HandlerLevelEarthsState();
        SetFirstLevelUI();
    }

    private void HandlerLevelEarthsState()
    {
        foreach (var level in _levelEarths)
        {
            level.SetStateLevel(level.StateLevel);
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