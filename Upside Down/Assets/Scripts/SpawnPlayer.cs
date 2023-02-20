using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    // called through event and used to instantiate a player when level is shifted (in LevelShiftTrigger script).
    public void Spawn()
    {
        Instantiate(_playerPrefab, gameObject.transform.position, Quaternion.identity);
    }
}
