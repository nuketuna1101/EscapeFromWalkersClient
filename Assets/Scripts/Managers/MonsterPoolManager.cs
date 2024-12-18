using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class MonsterPoolManager : Singleton<MonsterPoolManager>
{
    [SerializeField]
    private GameObject prefab;                                  // ������Ʈ ������
    [SerializeField]
    private const int initPoolSize = 5;                           // �ʱ� Ǯ ������ ����
    Queue<GameObject> pool = new Queue<GameObject>();              // ������ Ǯ�� �̿��� ť

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
        // ������ �ʱ� Ǯ������� �� �ʿ��ؼ� ��û�ϸ� ���� �����Ͽ� �߰�������,
        // ���� ���� ȯ�濡���� �� ������ 5���� ���͸� �����ϵ��� �߰� ������ ����
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
        // ������ �ʱ� Ǯ������� �� �ʿ��ؼ� ��û�ϸ� ���� �����Ͽ� �߰�������,
        // ���� ���� ȯ�濡���� �� ������ 5���� ���͸� �����ϵ��� �߰� ������ ����
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
        // ������Ʈ ��Ȱ��ȭ��Ű�� �ٽ� Ǯ�� ���ͽ�Ű��
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.pool.Enqueue(obj);
    }
}
