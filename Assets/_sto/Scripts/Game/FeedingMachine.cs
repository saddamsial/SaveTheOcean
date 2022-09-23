using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingMachine : MonoBehaviour
{
  public static Action<FeedingMachine> onPoped, onNotPoped;

  public static int layerMask = 1;

  List<int> foods = new List<int>();

  void Awake()
  {
    layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
  }
  void FillFood()
  {
    var foodChances = GameData.Econo.foodChances;
    int food_idx = GameData.Prefabs.ItemTypeFromKind(Item.Kind.Food);
    for(int q = 0; q < foodChances.Length; ++q)
    {
      int cnt = Mathf.RoundToInt(foodChances[q] * 10);
      int type = food_idx + q;
      for(int w = 0; w < cnt; ++w)
        foods.Add(type);
    }
    foods.shuffle(40);
  }

  public Item.ID? Pop()
  {
    Item.ID? id = null;
    if(GameState.Econo.CanSpendCoins(GameData.Econo.coinFeedCost))
    {
      if(foods.Count == 0)
        FillFood();
      GameState.Econo.coins -= GameData.Econo.coinFeedCost;
      var _id = new Item.ID(0, 0, Item.Kind.Food);
      _id.type = foods.first();
      id = _id;
      foods.RemoveAt(0);
      onPoped?.Invoke(this);
    }
    else
      onNotPoped?.Invoke(this);

    return id;
  }
  public Vector3 vpos => transform.position;
}
