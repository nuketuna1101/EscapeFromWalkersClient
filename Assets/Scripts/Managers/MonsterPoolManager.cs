using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class MonsterPoolManager : Singleton<MonsterPoolManager>
{
    [SerializeField]
    private GameObject prefab;                                  // 오브젝트 프리팹
    [SerializeField]
    private const int initPoolSize = 5;                           // 초기 풀 사이즈 정의
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
        // 원래는 초기 풀사이즈보다 더 필요해서 요청하면 새로 생성하여 추가하지만,
        // 현재 게임 환경에서는 한 씬에서 5개의 몬스터만 존재하도록 추가 생성은 없음
        if (Instance.pool.Count > 0)
        {
            var obj = Instance.pool.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        return null;
    }

    public static GameObject GetFromPool_LEGACY()       
    {
        // 원래는 초기 풀사이즈보다 더 필요해서 요청하면 새로 생성하여 추가하지만,
        // 현재 게임 환경에서는 한 씬에서 5개의 몬스터만 존재하도록 추가 생성은 없음
        if (Instance.pool.Count > 0)
        {
            var obj = Instance.pool.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateObj();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public static void ReturnToPool(GameObject obj)
    {
        // 오브젝트 비활성화시키고 다시 풀로 복귀시키기
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.pool.Enqueue(obj);
    }
}
