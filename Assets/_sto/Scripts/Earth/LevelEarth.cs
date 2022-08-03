using UnityEngine;

public class LevelEarth : MonoBehaviour
{
    [SerializeField] private GameObject _liningGameObject;
    [SerializeField] private int _indexLevel = 0; 

    public GameObject LiningGameObject => _liningGameObject;
    public int IndexLevel => _indexLevel; 
}
