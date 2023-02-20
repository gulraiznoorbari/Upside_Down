using UnityEngine;

public class SpawnCollectible : MonoBehaviour
{
    [SerializeField] private GameObject[] _spawnObjects;
    [SerializeField] private Transform[] _spawnLocations;

    private void Start()
    {
        for (int i = 0; i < _spawnLocations.Length; i++)
        {
            Instantiate(_spawnObjects[Random.Range(0, _spawnObjects.Length)], _spawnLocations[i]);
        }
    }
}
