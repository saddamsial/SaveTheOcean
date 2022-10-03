using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialSystem;

public class TutorialFeeding : Tutorial
{
    protected override bool IsTutorialCompleted() => false;     //change to feeding visited count > 0
}
