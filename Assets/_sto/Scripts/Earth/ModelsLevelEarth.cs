using UnityEngine;

public class ModelsLevelEarth : MonoBehaviour
{
    //Queue 1. Lock 2. Unlock 3. Passed 
    [SerializeField] private GameObject[] _gameObjectsModelsLevel;

    public GameObject[] GameObjectsModelsLevel => _gameObjectsModelsLevel;
}
