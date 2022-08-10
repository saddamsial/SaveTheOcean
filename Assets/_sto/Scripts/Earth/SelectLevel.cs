using UnityEngine;

[RequireComponent(typeof(EarthLevels))]
[DefaultExecutionOrder(-5)]
public class SelectLevel : MonoBehaviour
{
    [SerializeField] private Rotation _rotation;

    private Camera _camera;
    private EarthLevels _earthLevels;
    private LevelEarth _currentLevelEarth;
    private Vector3 _vec1;
    private Vector3 _vec2;

    private void Awake()
    {
      _camera = Camera.main;
        _earthLevels = GetComponent<EarthLevels>();
        _earthLevels.SetSelectLevel(this);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
            RaycastStart();
    }

    private void RaycastStart()
    {
    #if UNITY_EDITOR
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
    #else
        var ray = _camera.ScreenPointToRay(Input.GetTouch(0).position);
    #endif

        Physics.Raycast(ray, out var raycastHit);

        if (raycastHit.collider == null) return;

        var level = raycastHit.collider.GetComponentInParent<LevelEarth>();
        if (level)
        {
            SelectLevelEarth(level);
        }
    }

    public void SelectLevelEarth(LevelEarth level)
    {
        Select(level);
        var angle = GetReceiveAngleToSelectLevel(level);
        _rotation.RotateToSelectLevel(angle);
        _earthLevels.UIButtonEarth?.SetParametersLevelUI(level.IndexLevel, level.StateLevel);
    }

    private void Select(LevelEarth levelEarth)
    {
        Earth.onLevelSelected?.Invoke(levelEarth.IndexLevel);
      
        if (_currentLevelEarth == null)
        {
            _currentLevelEarth = levelEarth;
            _currentLevelEarth.LiningGameObject.SetActive(true);
        }
        else if (levelEarth != _currentLevelEarth)
        {
            _currentLevelEarth.LiningGameObject.SetActive(false);
            _currentLevelEarth = levelEarth;
            _currentLevelEarth.LiningGameObject.SetActive(true);
        }
    }

    public float GetReceiveAngleToSelectLevel(LevelEarth levelEarth)
    {
        _vec1 = _camera.transform.position - transform.position;
        _vec1 = Vector3.Normalize(Vector3.ProjectOnPlane(_vec1, Vector3.up));
        _vec2 = levelEarth.ModelTransform.position - transform.position;
        _vec2 = Vector3.Normalize(Vector3.ProjectOnPlane(_vec2, Vector3.up));
        var angle = Vector3.SignedAngle(_vec1, _vec2, Vector3.up);
        return angle;
    }
}