using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib;

public class StorageBox : MonoBehaviour
{
  [SerializeField] GameObject _infoContainer; 
  [SerializeField] TMPLbl     _lblCnt;
  [SerializeField] Collider   _collider;


  public static System.Action<StorageBox> onPushed, onPoped, onNotPoped, onNotPushed;

  public static int layerMask = 1;

  public enum PushState
  {
    Ok,
    Garbage,
    Food,
  }

  public PushState pushState {get; private set;} = PushState.Ok;

  void Awake()
  {
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

    UpdateInfo();

    this.Invoke(() => GetComponent<ActivatableObject>().ActivateObject(), 1.0f);
  }

  void UpdateInfo()
  {
    _infoContainer.SetActive(GameState.StorageBox.ItemsCnt() > 0);
    _lblCnt.text = $"x{GameState.StorageBox.ItemsCnt()}";
  }
  public bool IsStorageDrop(Collider col) => col == _collider;
  public void Push(Item.ID id)
  {
    pushState = PushState.Ok;
    GameState.StorageBox.PushItem(id);
    UpdateInfo();
    onPushed?.Invoke(this);
  }
  public void NoPush(Item.ID id)
  {
    if(id.kind == Item.Kind.Food)
      pushState = PushState.Food;
    else if(id.kind == Item.Kind.Garbage)
      pushState = PushState.Garbage;
    onNotPushed?.Invoke(this);
  }
  public Item.ID? Pop()
  {
    var id = GameState.StorageBox.PopItem();
    UpdateInfo();
    if(id!=null)
      onPoped?.Invoke(this);
    else
      onNotPoped?.Invoke(this);
      
    return id;
  }
}
