using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEarth : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;
    [SerializeField] private TextMeshProUGUI _textStateLevel;

    private int _currentSelectLevelIndex;
    private StateLevel _currentSelectState;

    private void Start()
    {
        _buttonPlay.onClick.AddListener(LoadLevel);
    }

    private void LoadLevel()
    {
        Debug.Log("Load Index Level:" + _currentSelectLevelIndex + " State level:" + _currentSelectState);
    }

    public void SetParametersLevelUI(int index, StateLevel state)
    {
        _textStateLevel.text = "Index Level:" + index + " State level:" + state;
        _currentSelectLevelIndex = index;
        _currentSelectState = state;

        _buttonPlay.interactable = state != StateLevel.Lock;
    }

    private void OnDisable()
    {
        _buttonPlay.onClick.RemoveAllListeners();
    }
}