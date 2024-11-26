using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab; // 풀링할 VFX 프리팹
    public int initialSize = 10; // 초기 생성할 객체 수

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // 초기화: 지정된 수만큼 객체 생성
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // 풀에서 객체를 가져옴
    public GameObject GetObject(Vector3 position, Quaternion rotation, float autoReturnTime = -1)
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            // 풀에 남는 객체가 없을 경우 새로 생성
            obj = Instantiate(prefab);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        // 자동 반환 시간 설정
        if (autoReturnTime > 0)
        {
            ObjectPoolTimer timer = obj.GetComponent<ObjectPoolTimer>();
            if (timer == null)
                timer = obj.AddComponent<ObjectPoolTimer>();

            timer.Initialize(this, obj, autoReturnTime);
        }

        return obj;
    }

    // 사용 완료된 객체를 풀에 반환
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}