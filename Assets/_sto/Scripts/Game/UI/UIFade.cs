using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
  [SerializeField] Image _image;
  [SerializeField] Color _fadeIn = new Color(1, 1, 1, 1);
  [SerializeField] Color _fadeOut = new Color(1, 1, 1, 0);

  Color _colorDst = new Color(1,1,1,0);
  float _t = 0;
  float _fadeSpeed = 3;

  public void BlendTo(Color color){ _t = 0; _colorDst = color;}
  public void FadeIn(float speed) {_fadeSpeed = speed; _t = 0; _colorDst = _fadeIn;}
  public void FadeOut(float speed) {_fadeSpeed = speed; _t = 0; _colorDst = _fadeOut;}
  void Awake()
  {
    _image.color = _fadeIn;
    FadeOut(_fadeSpeed);
  }
  void Update()
  {
    _image.color = Color.Lerp(_image.color, _colorDst, Time.deltaTime * _fadeSpeed);
  }
}
