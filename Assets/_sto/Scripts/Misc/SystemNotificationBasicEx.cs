using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GameLib.Notifications;

public class SystemNotificationBasicEx : SystemNotificationBasic
{
  static SystemNotificationBasicEx static_this = null;
  static SystemNotificationBasicEx get() => static_this;
  void Awake()
  {
    static_this = this;
  }

  public static float GetDisplayDelay() => get().displayDelay;
}
