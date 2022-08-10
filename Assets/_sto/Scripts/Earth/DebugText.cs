using System;
using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textDebug; 
    
    
    public static Action<string> OnDebugPrint;

    private void PrintText(string text) => _textDebug.text = text;

    private void OnEnable()
    {
        OnDebugPrint += PrintText;
    }

    private void OnDisable()
    {
        OnDebugPrint -= PrintText; 
    }
}
