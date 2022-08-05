using UnityEngine;

[RequireComponent(typeof(EarthLevels))]
public class Earth : MonoBehaviour
{
    private EarthLevels _earthLevels;

    private void Start()
    {
        _earthLevels = GetComponent<EarthLevels>();
    }

    public void Show(int indexLevel)
    {
        if (indexLevel < _earthLevels.LevelEarths.Length || indexLevel > _earthLevels.LevelEarths.Length)
        {
            Debug.LogError("Error index level");
            return;
        }
        
        _earthLevels.SelectLevel.SelectLevelEarth(_earthLevels.LevelEarths[0]);
    }
}
