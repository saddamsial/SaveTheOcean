using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib.UI;

public class UITutorial : MonoBehaviour
{
  [SerializeField] InputOverlayTutorial tutorial = null;


  public static System.Action onTutorialDone;

  List<Transform> listPositions = new List<Transform>();

  //Level level = null;
  
  void Awake()
  {
    Level.onCreate += OnLevelCreated;
    Level.onStart += OnTutorialStart;
    Level.onFinished += OnLevelFinished;
  }
  void OnDestroy()
  {
    Level.onCreate -= OnLevelCreated;
    Level.onStart -= OnLevelCreated;
    Level.onFinished -= OnLevelFinished;
  }
  void OnLevelCreated(Level lvl)
  {
    tutorial.Deactivate();
  }
  void OnTutorialStart(Level lvl)
  {
    if(lvl.locationIdx == 0)
    {
      //Vector3[] mm = new Vector3[]{lvl.garbagePosition(0), lvl.garbagePosition(1)};
      //tutorial.Activate(mm);
      //tutorial.Activate(new Vector3[]{new Vector3(-0.5f,0, 0), new Vector3(0.5f, 0, 0)});
    }
  }
  void OnLevelFinished(Level lvl)
  {
    tutorial.Deactivate();
  }
}
