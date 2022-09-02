using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPLbl = TMPro.TextMeshProUGUI;

public class StorageBox : MonoBehaviour
{
  [SerializeField] GameObject _infoContainer; 
  [SerializeField] TMPLbl     _lblCnt;
  [SerializeField] Collider   _collider;

  public static int layerMask = 1;

  void Awake()
  {
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

    UpdateInfo();
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
  }
  public Item.ID Pop()
  {
    var id = GameState.StorageBox.PopItem();
    UpdateInfo();
    return id;  
  }
}
