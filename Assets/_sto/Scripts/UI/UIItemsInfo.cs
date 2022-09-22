using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.UI;

public class UIItemsInfo : MonoBehaviour
{
  [SerializeField] Transform[] categories;
  bool setupDone = false;
  
  List<List<UIItemPreview>> _itemsPreview = new List<List<UIItemPreview>>();
  
  void Awake()
  {
    Setup();     
  }
  void Setup()
  {
    Item.ID id = new Item.ID(0,0, Item.Kind.Garbage);
    for(int q = 0; q < categories.Length; ++q)
    {
      _itemsPreview.Add(new List<UIItemPreview>());
      id.type = q;
      var itemsPreview = categories[q].GetComponentsInChildren<UIItemPreview>(true);
      for(int c = 0; c < itemsPreview.Length; ++c)
      {
        id.lvl = c;
        if(c < GameData.Prefabs.ItemLevelsCnt(id))
        {
          var item = GameData.Prefabs.CreateStaticItem(id, transform);
          itemsPreview[c].SetItem(item);
          _itemsPreview.last().Add(itemsPreview[c]);
        }
      }
    }
    setupDone = true;
  }
  public void Show()
  {
    if(!setupDone)
      Setup();

    for(int c = 0; c < _itemsPreview.Count; ++c)
    {
      for(int q = 0; q < _itemsPreview[c].Count; ++q)
      {
        _itemsPreview[c][q].SetLocked(!GameState.Progress.Items.DidItemAppear(_itemsPreview[c][q].id));
      }
    }

    GetComponent<UIPanel>().ActivatePanel();
  }
}
