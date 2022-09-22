using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GarbagePile : MonoBehaviour
{
    [SerializeField] Vector2 pileSize = Vector2.one;
    [SerializeField] GameObject elementPrefab = null;
    [SerializeField] Vector2 sizeRandom = Vector2.up; 
    [SerializeField] int pileQuanti = 32;
    
    List<GameObject> _pileContent = new List<GameObject>();
    int _lastItemIndex = 0;
    
    private void Start() {
        GeneratePile(pileQuanti);
    }

    private void OnEnable() {
        Level.onCreate += GeneratePile;
        Item.onShow += PopTrash;
    }
    private void OnDisable() {
        Level.onCreate -= GeneratePile;
        Item.onShow -= PopTrash;
    }
    void GeneratePile(Level sender) => GeneratePile(sender.GetNumberOfMovesToSolve());
    void PopTrash(object sender) => PopTrash();
    

    public void GeneratePile(int quantity){
        for (int i = 0; i < quantity; i++){
            var spawnOffset = Random.insideUnitSphere * pileSize.x;
            spawnOffset = new Vector3(spawnOffset.x, Mathf.Abs(spawnOffset.y * pileSize.y), spawnOffset.z);

            var newSpawn = Instantiate(
                elementPrefab, 
                transform.position + spawnOffset,
                Quaternion.LookRotation(Random.onUnitSphere),
                this.transform);
                newSpawn.transform.localScale = Vector3.one * Random.Range(sizeRandom.x, sizeRandom.y);
            _pileContent.Add(newSpawn);
        }
        _lastItemIndex = quantity;
    }

    public void PopTrash(){
        if (_lastItemIndex < 1) return;
        _pileContent[--_lastItemIndex].SetActive(false);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        GameLib.GizmosExtensions.DrawWireCircle(transform.position, pileSize.x, transform.up);
    }
}
