using UnityEngine;
using AngryKoala.Pooling;
using AngryKoala.Services;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] private float _spawnDelay;

    private int _cubeCounter;
    private float _spawnTimer;

    private IPoolService _poolService;

    private void Start()
    {
        _poolService = ServiceLocator.Get<IPoolService>();
    }

    private void Update()
    {
        if (_spawnTimer >= _spawnDelay)
        {
            if (_cubeCounter % 2 == 0)
            {
                PoolableCube poolableCube = _poolService.CubePool.Get(PoolKey.RedPoolableCube);
                poolableCube.transform.position = Vector3.up * 20f;
                poolableCube.transform.rotation = Quaternion.identity;
            }
            else
            {
                _poolService.CubePool.Get(PoolKey.BluePoolableCube, Vector3.up * 20f, Quaternion.identity);
            }

            _cubeCounter++;
            _spawnTimer = 0f;
        }
        else
        {
            _spawnTimer += Time.deltaTime;
        }
    }
}