using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEfxPoolManager : Singleton<HealEfxPoolManager>
{
    [SerializeField]
    private GameObject prefab;                                  // 오브젝트 프리팹
    [SerializeField]
    private const int initPoolSize = 4;                           // 초기 풀 사이즈 정의
    Queue<GameObject> pool = new Queue<GameObject>();              // 아이템 풀로 이용할 큐
    private void Awake()
    {
        InitPool();
    }
    public void InitPool()
    {
        for (int i = 0; i < initPoolSize; i++)
            pool.Enqueue(CreateObj());
    }
    private GameObject CreateObj()
    {
        var newObj = Instantiate(prefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }
    public static GameObject GetFromPool()
    {
        // 요청 시 풀에 있는 오브젝트를 할당해준다.
        if (Instance.pool.Count > 0)
        {
            var obj = Instance.pool.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            // 가진 풀보다 더 필요하면, 풀을 늘려 새로 생성하여 이용
            var newObj = Instance.CreateObj();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }
    public static void ReturnToPool(GameObject obj)
    {
        // 오브젝트 비활성화시키고 다시 풀로 복귀시키기
        //if (obj == null) return;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.pool.Enqueue(obj);
    }
}
