using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
  [SerializeField] Image _image;
  [SerializeField] float _fadeSpeed = 4;

  Color _colorDst = new Color(0,0,0,0);

  public void BlendTo(Color color) => _colorDst = color;
  public void SetColor(Color color) => _colorDst = _image.color = color;

  void Update()
  {
    _image.color = Color.Lerp(_image.color, _colorDst, Time.deltaTime * _fadeSpeed);
  }
}
