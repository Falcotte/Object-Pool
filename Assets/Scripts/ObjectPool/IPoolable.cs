namespace AngryKoala.ObjectPool
{
    public interface IPoolable
    {
        // Called when the object gets pulled from the pool
        void Initialize();

        // Called when the object gets returned to the pool
        void Terminate();
    }
}