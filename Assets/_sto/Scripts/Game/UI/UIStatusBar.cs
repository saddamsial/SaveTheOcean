using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPLbl = TMPro.TextMeshProUGUI;
using GameLib.UI;

public class UIStatusBar : MonoBehaviour
{
  [Header("Refs")]
  [SerializeField] UIInfoBox _stamina;
  [SerializeField] UIInfoBox _coins;
  [SerializeField] UIInfoBox _gems;
  [SerializeField] Transform _movesContainer;
  [Header("Settings")]
  [SerializeField] float _moveSpeed = 5.0f;
  [SerializeField] float _moveDelay = 0.05f;

  int _staminaDisp = 0;
  int _coinsDisp = 0;
  int _gemsDisp = 0;

  struct Move
  {
    public Vector3          vdst;
    public List<GameObject> objects;
    public List<float>      delays;
  }
  List<Move> moves = new List<Move>();

  void Awake()
  {
    GameState.Econo.onStaminaChanged += OnStaminaChanged;//OnStaminaChanged;
    GameState.Econo.onCoinsChanged += OnCoinsChanged; //OnCoinsChanged;
    GameState.Econo.onGemsChanged += OnGemsChanged;// OnGemsChanged;

    SetupRes();
  }
  void OnDestroy()
  {
    GameState.Econo.onStaminaChanged += OnStaminaChanged;//OnStaminaChanged;
    GameState.Econo.onCoinsChanged += OnCoinsChanged; //OnCoinsChanged;
    GameState.Econo.onGemsChanged += OnGemsChanged;// OnGemsChanged;
  }

  public void Show()
  {
    GetComponent<UIPanel>().ActivatePanel();
    SetupRes();
  }
  void SetupRes()
  {
    _staminaDisp = GameState.Econo.stamina;
    _coinsDisp = GameState.Econo.coins;
    _gemsDisp = GameState.Econo.gems;
    UpdateResDisp();
  }
  void UpdateDisp()
  {
    _staminaDisp = (int)Mathf.MoveTowards(_staminaDisp, GameState.Econo.stamina, 1);
    _coinsDisp = (int)Mathf.MoveTowards(_coinsDisp, GameState.Econo.coins, 1);
    _gemsDisp = (int)Mathf.MoveTowards(_gemsDisp, GameState.Econo.gems, 1);
    UpdateResDisp();
  }
  void UpdateResDisp()
  {
    _stamina.resValue = _staminaDisp; //GameState.Econo.stamina;
    _gems.resValue = _gemsDisp; //GameState.Econo.gems;
    _coins.resValue = _coinsDisp; //GameState.Econo.coins;
  }

  void OnStaminaChanged(int val) => SetupRes();
  void OnGemsChanged(int val) => SetupRes();
  void OnCoinsChanged(int val) => SetupRes();

  public void MoveCollected(Item item, int amount)
  {
    Move move = new Move();
    Vector3 vdstPos  = item.id.kind switch
    {
      Item.Kind.Stamina => _stamina.transform.position,
      Item.Kind.Coin => _coins.transform.position,
      Item.Kind.Gem => _gems.transform.position,
      _ => Vector3.zero
    };

    float dist = Mathf.Abs(Camera.main.transform.position.z - vdstPos.z);
    move.vdst = Camera.main.ScreenToWorldPoint(new Vector3(vdstPos.x, vdstPos.y, dist));// -Camera.main.nearClipPlane + dist + 32));
    move.objects = new List<GameObject>();
    move.delays = new List<float>();
    move.objects.AddRange(GameData.Prefabs.CreateStaticItemModels(item.id, _movesContainer, amount));
    for(int q = 0; q < amount; ++q)
    {
      move.objects[q].transform.position = item.vwpos;
      move.delays.Add(-_moveDelay * q);
    }
    moves.Add(move);
  }
  void ProcessMoves()
  {
    for(int q = 0; q < moves.Count;)
    {
      Move move = moves[q];
      for(int o = 0; o < move.objects.Count;)
      {
        move.delays[o] += Time.deltaTime;
        if(move.delays[o] > 0)
        {
          move.objects[o].transform.position = Vector3.Lerp(move.objects[o].transform.position, move.vdst, Time.deltaTime * _moveSpeed);
          if(Vector3.Distance(move.objects[o].transform.position, move.vdst) < 0.33f)
          {
            Destroy(move.objects[o].gameObject);
            move.objects.RemoveAt(o);
            move.delays.RemoveAt(o);
            --o;
            UpdateDisp();
          }
        }
        ++o;
      }
      if(move.objects.Count > 0)
        moves[q] = move;
      else
      {
        moves.RemoveAt(q);
        --q;
      }
      ++q;
    }
  }

  void Update()
  {
    ProcessMoves();
    float perc = GameState.Econo.GetStaminaRefillPerc();
    _stamina.progressVal = perc;
  }
}

