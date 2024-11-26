using UnityEngine;

public class ObjectPoolTimer : MonoBehaviour
{
    private ObjectPool pool;
    private GameObject pooledObject;
    private float timer;
    private bool isTimerActive;

    public void Initialize(ObjectPool pool, GameObject obj, float duration)
    {
        this.pool = pool;
        this.pooledObject = obj;
        this.timer = duration;
        this.isTimerActive = true;
    }

    void Update()
    {
        if (isTimerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isTimerActive = false;
                pool.ReturnObject(pooledObject);
            }
        }
    }
}
