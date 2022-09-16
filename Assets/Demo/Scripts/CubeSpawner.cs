using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AngryKoala.ObjectPool;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private float spawnDelay;
    private float spawnTimer;

    private void Update()
    {
        if(spawnTimer >= spawnDelay)
        {
            CubePool.Instance.GetPooledObject(Vector3.up * 4f, Quaternion.identity);
            spawnTimer = 0f;
        }
        else
        {
            spawnTimer += Time.deltaTime;
        }
    }
}
