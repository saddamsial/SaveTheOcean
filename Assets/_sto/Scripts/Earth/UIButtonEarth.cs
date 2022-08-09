using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEarth : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;

    private int _currentSelectLevelIndex;
    private StateLevel _currentSelectState;
    public static Action OnEnableButton;

    private void Awake()
    {
        OnEnableButton += EnableButton;
    }
    private void Start()
    {
        _buttonPlay.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        _buttonPlay.gameObject.SetActive(false);
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

    private void EnableButton()
    {
//        Debug.Log("Activate Button");
        _buttonPlay?.gameObject.SetActive(true);
    }



    private void OnDisable()
    {
        OnEnableButton -= EnableButton;

        _buttonPlay.onClick.RemoveAllListeners();
    }
}