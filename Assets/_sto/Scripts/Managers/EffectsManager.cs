using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameLib.Utilities;
using GameLib.UI;

public class EffectsManager : MonoBehaviour
{
  public static System.Action onPlayConfetti;

  [ContextMenu("Test Confetti")] void TestConfetti() => onPlayConfetti?.Invoke();
  Camera mainCamera = null;
  ObjectShake cameraShakeContainer;
  UIInfoLabelManager infoLblMan,infoLblManDown;

  [Header("CamFX")]
    [SerializeField] ObjectShakePreset objShakePreset;
    [SerializeField] ObjectShakePreset objShakePresetLo;
    [SerializeField] ObjectShakePreset objShakePresetHi;
    [SerializeField] float offsetToCamera = .25f;
  [Header("FX Systems")]
    //[SerializeField] ParticleSystem fxSparks = null;
    //[SerializeField] ParticleSystem fxItemCompleted = null;
    //[SerializeField] ParticleSystem fxConfettiIngame = null;
    [SerializeField] ParticleSystem fxConfettiLevel = null;
    [SerializeField] ParticleSystem fxPaintSplat = null;
//    [SerializeField] int ballFracturesEmitCnt = 1;
    //[SerializeField] ParticleSystem fxBallFracturesSub = null;
    //[SerializeField] int ballFracturesSubEmitCnt = 1;
    [SerializeField] ParticleSystem fxHit = null;
    //[SerializeField] int fxBombDestroyEmitCnt = 5;

    [Header("FX string")]
    [SerializeField] string _strNoMergeMaxed;
    [SerializeField] string _strNoMergeWrongType;
    [SerializeField] string _strAnimalWrongItem;
    [SerializeField] string _strNoCapacity;
    [SerializeField] string _strNoSplittableItem;

    List<GameLib.ObjectFracture> listFractures = new List<GameLib.ObjectFracture>();

    private void Awake() 
    {
      mainCamera = Camera.main;
      cameraShakeContainer = Camera.main.GetComponentInParent<ObjectShake>();
      infoLblMan = FindObjectOfType<UIInfoLabelManager>(true);
      //infoLblManDown = GameObject.Find("infoCanvas2").GetComponent<UIInfoLabelManager>();
    }
    private void OnEnable() 
    {
      Level.onStart += OnLevelStart;
      Level.onDone += OnLevelDone;
      Level.onFinished += OnLevelFinished;

      Item.onShown += OnItemShown;
      Item.onMerged += OnItemMerged;
      Item.onNoMerged += OnItemNoMerged;
      Item.onPut += OnItemPut;
      Item.onNoPut += OnItemNoPut;

      SplitMachine.onDropped += OnSplitMachineDrop;
    }
    private void OnDisable()
    {
      Level.onStart -= OnLevelStart;
      Level.onDone -= OnLevelDone;
      Level.onFinished -= OnLevelFinished;

      Item.onShown -= OnItemShown;
      Item.onMerged -= OnItemMerged;
      Item.onNoMerged -= OnItemNoMerged;
      Item.onPut -= OnItemPut;
      Item.onNoPut -= OnItemNoPut;

      SplitMachine.onDropped -= OnSplitMachineDrop;
    }

    Vector3 GetFxPosition(Vector3 objectPosition) => objectPosition + (objectPosition - Camera.main.transform.position).normalized * -offsetToCamera;
    void PlayFXAtPosition(ParticleSystem ps, Vector3 worldPosition, int emitCount = 0, bool useCameraOffset = true)
    {
      ps.transform.position = useCameraOffset ? GetFxPosition(worldPosition) : worldPosition;
      if(emitCount > 0)
        ps.Emit(emitCount);
      else
        ps.Play(true);
    }
    void PlayFXAtPosition(ParticleSystem ps, ParticleSystem[] subs, Vector3 worldPosition, int emitCount = 0, bool useCameraOffset = true)
    {
      ps.transform.position = useCameraOffset ? GetFxPosition(worldPosition) : worldPosition;
      if(emitCount > 0)
      {
        ps.Emit(emitCount);
        foreach(var sub in subs)
        {
          sub.transform.position = ps.transform.position;
          sub.Emit(emitCount);
        }
      }
      else
        ps.Play(true);
    }    
    
    void OnLevelStart(Level lvl)
    {
      //_lvl = lvl;
    }

    void OnItemMerged(Item sender)
    {
      //PlayFXAtPosition(fxHit, sender.transform.position);
      PlayFXAtPosition(fxPaintSplat, sender.transform.position, 0);
    }
    void OnItemNoMerged(Item sender)
    {
      if(sender.mergeType == Item.MergeType.RejectMaxed)
        infoLblMan.ShowTextPopup(sender.vwpos, _strNoMergeMaxed);
      else if(sender.mergeType == Item.MergeType.RejectWrongType)
        infoLblMan.ShowTextPopup(sender.vwpos, _strNoMergeWrongType);
    }    
    void OnItemShown(Item sender)
    {
      //var psmain = fxPaintSplat.main;
      //psmain.startColor = sender.color;
      PlayFXAtPosition(fxPaintSplat, sender.gridPos, 0, false);
    }
    void OnItemPut(Item sender)
    {
      PlayFXAtPosition(fxHit, sender.transform.position, 0, false);
    }
    void OnItemNoPut(Item sender)
    {
      infoLblMan.ShowTextPopup(sender.vwpos, _strAnimalWrongItem);
    }
    void OnSplitMachineDrop(SplitMachine sm)
    {
      if(sm.dropResult == SplitMachine.DropResult.NoSplittableItem)
        infoLblMan.ShowTextPopup(sm.dropPosition, _strNoSplittableItem);
      else if(sm.dropResult == SplitMachine.DropResult.NoCapacity)
        infoLblMan.ShowTextPopup(sm.dropPosition, _strNoCapacity);
    }


    void OnItemExplo(Item sender)
    {
      //infoLblMan.ShowTextPopup(sender.transform.position, string.Format(strItemExplo, sender.Points), sender.color);
    }
    // void OnItemsHit(Item itemA, Item itemB)
    // {
    //   if(itemA && itemB)
    //     PlayFXAtPosition(fxHit, (itemA.transform.position + itemB.transform.position) * 0.5f, 30);
    // }
    // void OnItemPainted(Item sender)
    // {
    //   var psmain = fxPaintSplat.main;
    //   psmain.startColor = sender.color;
    //   PlayFXAtPosition(fxPaintSplat, sender.transform.position, 1);      
    // }
    // void OnItemsMatched(Level.Match3 match)
    // {
    //   Vector3 v = match.MidPos();
    //   v += new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f));
    //   //PlayFXAtPosition(fxPaintSplat, v, 5);
    //   infoLblMan.ShowTextPopup(v, string.Format(strBallsMatched, match.Points), match.GetColor());
    //   cameraShakeContainer.Shake(objShakePresetLo);
    // }
    void OnItemDestroy(Item sender)
    {
      // if(!sender.IsPainter)
      // {
      //   var psmain = fxBallFractures.main;
      //   psmain.startColor = sender.color;
      //   PlayFXAtPosition(fxBallFractures, sender.transform.position, ballFracturesEmitCnt, false);
      //   psmain = fxBallFracturesSub.main;
      //   psmain.startColor = sender.color;
      //   PlayFXAtPosition(fxBallFracturesSub, sender.transform.position, ballFracturesSubEmitCnt, false);
      // }
      // else
      // {
      //   var psmain = fxPainter.main;
      //   psmain.startColor = sender.color;
      //   PlayFXAtPosition(fxPainter, fxPainterSubs, sender.transform.position, 1);
      // }
      //var emitParams = new ParticleSystem.EmitParams();
      //emitParams.position = sender.transform.position;
      //fxBallFractures.Emit(emitParams, 4);
    }
    void OnMoveLblChanged(GameObject go)
    {
      //infoLblMan.ShowTextPopupUI(go.transform.position + new Vector3(Random.Range(-1.0f, 1.0f) * 100, 0,Random.Range(-1.0f, 1.0f)*100) , "asdad");
      //infoLblManDown.ShowTextPopupUI(go.transform.position + new Vector3(Random.Range(-1.0f, 1.0f) * 25, 0, Random.Range(-1.0f, 1.0f) * 10), string.Format("+{0}", GameData.Points.moveLeft));
    }
    void OnFx00(object sender)
    {
      //PlayFXAtPosition(fxConfettiIngame, sender.transform.position);
      //PlayFXAtPosition(fxSparks, sender.transform.position);
      //PlayFXAtPosition(fxItemCompleted, sender.transform.position);
      //infoLblMan.ShowTextPopup(sender.transform.position, sender.GetSatisfactionString());
    }
    void OnFx01(object sender)
    {
      //infoLblMan.ShowTextPopup(sender.transform.position, customerWrongItemType, Color.red);
    }
    void OnFx02(object sender)
    {
      //infoLblMan.ShowTextPopup(item.vPos, GameData.Satisfy.GetSatisfyString(3), Color.green);
      //SparksFX(item.vPos);
    }
    void OnLevelCombo(int events_cnt)
    {
      //infoLblBigMan.ShowTextPopup(Vector3.zero, strGreetings.get_random(), Color.white);
    }
    void OnLevelDone(Level lvl)
    {
      if(lvl.succeed)
        onPlayConfetti?.Invoke();
    }
    void OnLevelFinished(Level lvl) 
    {
      // if(lvl.Succeed)
      //   fxConfettiLevel.Play();
      //cameraShakeContainer.Shake(objShakePresetHi);
      //_lvl = null;
    }
}
