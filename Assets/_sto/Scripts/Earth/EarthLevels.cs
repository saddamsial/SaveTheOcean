using System.Linq;
using UnityEngine;

public class EarthLevels : MonoBehaviour
{
    [SerializeField] private UIButtonEarth _uiButtonEarth;
    [SerializeField] private LevelEarth[] _levelEarths;
    
    private LevelEarth _currentUnlockLevel;
    private LevelEarth _currentPassedLevel;

    public UIButtonEarth UIButtonEarth => _uiButtonEarth;

    private void Start()
    {
   //     _levelEarths = GetComponentsInChildren<LevelEarth>();
        HandlerLevelEarthsState();
        SetFirstLevelUI();
        gameObject.GetComponentsInChildren<Transform>();
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

        if (_currentUnlockLevel == null)
        {
            _currentPassedLevel = _levelEarths.FirstOrDefault(level => level.StateLevel == StateLevel.Passed);
            _uiButtonEarth.SetParametersLevelUI(_currentPassedLevel.IndexLevel, _currentPassedLevel.StateLevel);
            return;
        }
        
        _uiButtonEarth.SetParametersLevelUI(_currentUnlockLevel.IndexLevel, _currentUnlockLevel.StateLevel);
    }
}