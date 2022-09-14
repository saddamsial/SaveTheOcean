using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingMachine : MonoBehaviour
{
  public static Action<FeedingMachine> onPoped, onNotPoped;

  public Item.ID? Pop()
  {
    Item.ID? id = null;
    if(GameState.Econo.CanSpendCoins(GameData.Econo.coinFeedCost))
    {
      GameState.Econo.coins -= GameData.Econo.coinFeedCost;
      id = new Item.ID(0, 0, Item.Kind.Food);
      onPoped?.Invoke(this);
    }
    else
      onNotPoped?.Invoke(this);

    return id;
  }
  public Vector3 vpos => transform.position;
}
