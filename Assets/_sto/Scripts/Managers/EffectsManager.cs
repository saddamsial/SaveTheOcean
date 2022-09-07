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
      //Level.onDone += OnLevelDone;
      Level.onFinished += OnLevelFinished;

      Item.onShown += OnItemShown;
      Item.onHide += OnItemHide;
      Item.onMerged += OnItemMerged;
      Item.onNoMerged += OnItemNoMerged;
      Item.onPut += OnItemPut;
      Item.onNoPut += OnItemNoPut;

      SplitMachine.onSplitted += OnSplitMachineSplitted;
      SplitMachine.onDropped += OnSplitMachineDrop;
    }
    private void OnDisable()
    {
      Level.onStart -= OnLevelStart;
      //Level.onDone -= OnLevelDone;
      Level.onFinished -= OnLevelFinished;

      Item.onShown -= OnItemShown;
      Item.onHide -= OnItemHide;
      Item.onMerged -= OnItemMerged;
      Item.onNoMerged -= OnItemNoMerged;
      Item.onPut -= OnItemPut;
      Item.onNoPut -= OnItemNoPut;

      SplitMachine.onSplitted -= OnSplitMachineSplitted;
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
      PlayFXAtPosition(fxPaintSplat, sender.gridPos, 0, false);
    }
    void OnItemHide(Item sender)
    {

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
    void OnSplitMachineSplitted(SplitMachine sm)
    {
      //else if(sm.dropResult == SplitMachine.DropResult.Ok)
      PlayFXAtPosition(fxHit, sm.dropPosition, 50, false);
      PlayFXAtPosition(fxHit, sm.GetSplitSlotPos(0), 50, false);
      PlayFXAtPosition(fxHit, sm.GetSplitSlotPos(1), 50, false);
    }

    void OnItemDestroy(Item sender)
    {

    }
    void OnLevelFinished(Level lvl) 
    {
      if(lvl.succeed)
        onPlayConfetti?.Invoke();
      // if(lvl.Succeed)
      //   fxConfettiLevel.Play();
      //cameraShakeContainer.Shake(objShakePresetHi);
    }
}
