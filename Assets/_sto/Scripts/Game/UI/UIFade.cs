using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
  [SerializeField] Image _image;
  [SerializeField] float _fadeSpeed = 4;

  [SerializeField] Color _fadeIn = new Color(1, 1, 1, 1);
  [SerializeField] Color _fadeOut = new Color(1, 1, 1, 0);

  Color _colorDst = new Color(1,1,1,0);
  float _t = 0;

  public void BlendTo(Color color){ _t = 0; _colorDst = color;}
  public void FadeIn() { _t = 0; _colorDst = _fadeIn;}
  public void FadeOut() { _t = 0; _colorDst = _fadeOut;}
  //public void SetColor(Color color) => _colorDst = _image.color = color;
  void Awake()
  {
    _image.color = _fadeIn;
    FadeOut();
  }
  void Update()
  {
    // _t = Mathf.Clamp01(_t + Time.deltaTime * _fadeSpeed);
    // _image.color = Color.Lerp(_image.color, _colorDst, _t);
    _image.color = Color.Lerp(_image.color, _colorDst, Time.deltaTime * _fadeSpeed);
  }
}
