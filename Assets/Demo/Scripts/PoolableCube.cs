using UnityEngine;
using AngryKoala.ObjectPool;

public class PoolableCube : MonoBehaviour, IPoolable
{
    public void Initialize()
    {
        CubePool.Instance.ReturnToPool(this, 2f);
    }

    public void Terminate()
    {

    }
}
