using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIItemPreview : MonoBehaviour
{
  [SerializeField] GameObject _itemContainer;
  [SerializeField] GameObject _itemContainerLocked;
  Item _item = null;

  public void SetItem(Item item)
  {
    _item = item;
    _item.transform.parent = _itemContainer.transform;
    _item.transform.localPosition = Vector3.zero;
    _item.transform.localScale = Vector3.one;
    _item.transform.localRotation = Quaternion.identity;
    _item.mdl.gameObject.layer = LayerMask.NameToLayer("UI");
  }
  public void SetLocked(bool locked)
  {
    _itemContainer.SetActive(!locked);
    _itemContainerLocked.SetActive(locked);
  }
  public bool visible {get => gameObject.activeSelf; set => gameObject.SetActive(value);}
  public Item.ID id => _item.id;
}
