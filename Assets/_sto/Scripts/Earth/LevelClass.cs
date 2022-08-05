using UnityEngine;

public class LevelClass : MonoBehaviour
{
    private int _index;
    private StateLevel _stateLevel = StateLevel.Lock;

    public int Index => _index;
    public StateLevel StateLevel => _stateLevel;

    public void SetParameters(int index, StateLevel state)
    {
        _index = index;
        _stateLevel = state; 
    }
}
