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


  public static System.Action<StorageBox> onPushed, onPoped, onNotPoped;

  public static int layerMask = 1;

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
    GameState.StorageBox.PushItem(id);
    UpdateInfo();
    onPushed?.Invoke(this);
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
