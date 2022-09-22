using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GarbagePile : MonoBehaviour
{
    [SerializeField] Vector2 pileSize = Vector2.one;
    [SerializeField] GameObject elementPrefab = null;
    [SerializeField] Vector2 sizeRandom = Vector2.up; 
    
    Queue<GameObject> _pileQueue = new Queue<GameObject>();
    
    private void OnEnable() {
        Level.onStart += GeneratePile;
        Level.onItemCleared += PopTrash;
    }
    private void OnDisable() {
        Level.onStart -= GeneratePile;
        Level.onItemCleared -= PopTrash;
    }
    void GeneratePile(Level sender) {
        GeneratePile(sender.GetNumberOfMovesToSolve());
    }
    void PopTrash(object sender) => PopTrash();


    public void GeneratePile(int quantity){
        _pileQueue = new Queue<GameObject>();

        for (int i = 0; i < quantity; i++){
            var spawnOffset = Random.insideUnitSphere * pileSize.x;
            spawnOffset = new Vector3(spawnOffset.x, Mathf.Abs(spawnOffset.y * pileSize.y), spawnOffset.z);

            var newSpawn = Instantiate(
                elementPrefab, 
                transform.position + spawnOffset,
                Quaternion.LookRotation(Random.onUnitSphere),
                this.transform);
                newSpawn.transform.localScale = Vector3.one * Random.Range(sizeRandom.x, sizeRandom.y);
            _pileQueue.Enqueue(newSpawn);
        }
    }

    public void PopTrash(){
        if (_pileQueue.TryDequeue(out GameObject lastObject))
            lastObject.gameObject.SetActive(false);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        GameLib.GizmosExtensions.DrawWireCircle(transform.position, pileSize.x, transform.up);
    }
}
