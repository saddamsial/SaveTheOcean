using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonDebug : MonoBehaviour
{
    [SerializeField] private Button _buttonDebug;
    [SerializeField] private GameObject _gameObjectDebugPanel; 
    
    private bool _isOpenPanelDebug;

    private void Start()
    {
        _isOpenPanelDebug = _gameObjectDebugPanel.activeSelf;
    }

    private void EnableAndDisablePanelDebug()
    {
        _isOpenPanelDebug = !_isOpenPanelDebug; 
        _gameObjectDebugPanel.SetActive(_isOpenPanelDebug);
    }

    private void OnEnable()
    {
        _buttonDebug.onClick.AddListener(EnableAndDisablePanelDebug);
    }

    private void OnDisable()
    {
        _buttonDebug.onClick.RemoveAllListeners();
    }
}
