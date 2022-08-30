using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIStatusBar : MonoBehaviour
{
  [SerializeField] UIInfoBox _stamina;
  [SerializeField] UIInfoBox _coins;
  [SerializeField] UIInfoBox _gems;

  void Awake()
  {
    GameState.Econo.onStaminaChanged += OnResChanged;//OnStaminaChanged;
    GameState.Econo.onGemsChanged += OnResChanged;// OnGemsChanged;
    GameState.Econo.onCoinsChanged += OnResChanged; //OnCoinsChanged;

    _stamina.resValue = GameState.Econo.coins;
    _gems.resValue = GameState.Econo.gems;
    _coins.resValue = GameState.Econo.coins;
  }
  void OnDestroy()
  {
    GameState.Econo.onStaminaChanged -= OnResChanged;//StaminaChanged;
    GameState.Econo.onGemsChanged -= OnResChanged; //OnGemsChanged;
    GameState.Econo.onCoinsChanged -= OnResChanged; //OnCoinsChanged;
  }

  public void Show()
  {
    GetComponent<UIPanel>().ActivatePanel();
    UpdateRes();
  }
  void OnResChanged(int val)
  {
    UpdateRes();
  }
  void UpdateRes()
  {
    _stamina.resValue = GameState.Econo.stamina;
    _gems.resValue = GameState.Econo.gems;
    _coins.resValue = GameState.Econo.coins;
  }

  void OnStaminaChanged(int val) => UpdateRes();
  void OnGemsChanged(int val) => UpdateRes();
  void OnCoinsChanged(int val) => UpdateRes();

  void Update()
  {
    float perc = GameState.Econo.GetStaminaRefillPerc();
    _stamina.progressVal = perc;
  }
}

