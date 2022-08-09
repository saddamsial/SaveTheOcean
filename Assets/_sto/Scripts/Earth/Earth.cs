using UnityEngine;

public class Earth : MonoBehaviour
{

  [SerializeField] GameObject _antonEarth;

  public static System.Action<int> onShow;
  public static System.Action onHide;
  public static System.Action<int> onLevelStart, onLevelSelected;

  public void Show(int indexLevel)
  {
    _antonEarth.SetActive(true);
    onShow?.Invoke(indexLevel);
  }
  public void Hide()
  {
    _antonEarth.SetActive(false);
    onHide?.Invoke();
  }
}
