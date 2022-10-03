using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TutorialCharacterName : MonoBehaviour
{
    public enum CharacterNames { Dolphin, Tutoise, HammerShark, Octopus }
    string[] _displayNames = new string[]{ "None", "Dolph-E", "Terr-E", "Hoom-E", "Octo-E" };
    [SerializeField] CharacterNames activeCharacter = CharacterNames.Dolphin;
    [SerializeField] TextMeshProUGUI nameLabel = null;

    private void Awake() {
        nameLabel.text = _displayNames[(int)activeCharacter];
    }
}
