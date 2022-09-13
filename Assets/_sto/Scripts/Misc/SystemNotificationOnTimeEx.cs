using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameLib.Notifications;

public class SystemNotificationOnTimeEx : SystemNotificationOnTime
{
  public static List<DateTime> dateTimes {get; private set;} = new List<DateTime>();
  
  void Awake()
  {
    dateTimes.Add(this.DisplayTime);
  }
  //public static DateTime GetDisplayTime() => get().DisplayTime;
}
