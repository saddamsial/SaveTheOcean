using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEarth : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;

    private int _currentSelectLevelIndex;
    private StateLevel _currentSelectState;

    private void LoadLevel()
    {
      Earth.onLevelStart?.Invoke(_currentSelectLevelIndex);
      Debug.Log("Load Index Level:" + _currentSelectLevelIndex + " State level:" + _currentSelectState);
    }

    public void SetParametersLevelUI(int index, StateLevel state)
    {
        //       Debug.Log("Index Level:" + _currentSelectLevelIndex + " State level:" + _currentSelectState);
        _currentSelectLevelIndex = index;
        _currentSelectState = state;

        _buttonPlay.interactable = state != StateLevel.Lock;
    }

    public void OnBtnPlay()
    {
      LoadLevel();
    }
}