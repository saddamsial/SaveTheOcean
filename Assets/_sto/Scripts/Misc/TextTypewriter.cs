using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextTypewriter : MonoBehaviour
{
    [SerializeField] bool autoStart = true;
    [SerializeField] float initialDelay = 1f;
    TextMeshProUGUI textField = null;
    int progressIndex = 0;
    int messageLength = 0;
    int frameCounter = 0;
    [SerializeField] int typewriterSpeed = 5;

    void Awake()
    {
        textField = GetComponent<TextMeshProUGUI>();
        // textField.maxVisibleWords = 0;
        // wordsInMessage = textField.GetTextInfo(textField.text).wordCount;
        textField.maxVisibleCharacters = 0;
        messageLength = textField.text.Length;
        
        if(!autoStart) this.enabled = false;
    }
    IEnumerator Start(){
        yield return new WaitForSeconds(initialDelay);
        this.enabled = true;
    }
    void FixedUpdate()
    {
        if (progressIndex > messageLength) {
            Debug.Log("Text printed | " + progressIndex + "/" + messageLength);
            this.enabled = false;
            return;
        }
        if (frameCounter%typewriterSpeed != 0) return;
        // textField.maxVisibleWords = ++progressIndex;
        textField.maxVisibleCharacters = ++progressIndex;
    }
}
