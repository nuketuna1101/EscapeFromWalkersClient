using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHPbarPoolManager : Singleton<PlayerHPbarPoolManager>
{
    /// <summary>
    /// ���Ϳ��� �޾��� HP ü�¹� UI
    /// </summary>
    [SerializeField]
    private GameObject prefab;                                  // ������Ʈ ������
    [SerializeField]
    private const int initPoolSize = 4;                           // �ʱ� Ǯ ������ ����
    Queue<GameObject> pool = new Queue<GameObject>();              // ������ Ǯ�� �̿��� ť

    private new void Awake()
    {
        base.Awake();
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
        // ��û �� Ǯ�� �ִ� ������Ʈ�� �Ҵ����ش�.
        if (Instance.pool.Count > 0)
        {
            var obj = Instance.pool.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            // ���� Ǯ���� �� �ʿ��ϸ�, Ǯ�� �÷� ���� �����Ͽ� �̿�
            var newObj = Instance.CreateObj();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }
    public static void ReturnToPool(GameObject obj)
    {
        // ������Ʈ ��Ȱ��ȭ��Ű�� �ٽ� Ǯ�� ���ͽ�Ű��
        //if (obj == null) return;
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.pool.Enqueue(obj);
    }
}
