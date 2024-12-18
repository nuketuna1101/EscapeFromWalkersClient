using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Sprite[] sprites = new Sprite[4];
    private Transform CharacterSlotsParent;
    [SerializeField]
    private GameObject popupWindowPrefab;
    private Queue<GameObject> readyQueue = new Queue<GameObject>();              // ������ Ǯ�� �̿��� ť


    private new void Awake()
    {
        base.Awake();
        Canvas canvas = FindObjectOfType<Canvas>();
        CharacterSlotsParent = canvas.transform.GetChild(0).GetChild(1).transform;
        for(int i = 0; i < 4; i++)
        {
            Transform slot = CharacterSlotsParent.GetChild(i).transform;
            slot.GetChild(1).GetComponent<Image>().sprite = sprites[i];
            slot.GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = string.Format("{0}", (i + 1));
            Slider slider = slot.GetChild(3).GetComponent<Slider>();
            slider.maxValue = BattleManager.Instance.Players[i].maxHealth;
            slider.value = BattleManager.Instance.Players[i].health;
        }
        InitPopupWindow();
    }
    private void InitPopupWindow()
    {
        var obj = Instantiate(popupWindowPrefab);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        readyQueue.Enqueue(obj);
    }
    public GameObject GetPopupWindow()
    {
        if (readyQueue.Count <= 0) return null;
        var obj = readyQueue.Dequeue();
        obj.transform.position = Camera.main.WorldToScreenPoint(Vector3.zero);
        Canvas canvas = FindObjectOfType<Canvas>();
        obj.transform.SetParent(canvas.transform);
        obj.gameObject.SetActive(true);
        return obj;
    }
    public void RemovePopupWindow(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        readyQueue.Enqueue(obj);
    }
    public void ShowPopupWindow(string msg)
    {
        DebugOpt.Log("UIManager : ShowPopupWindow");
        var obj = GetPopupWindow();
        obj.transform.GetChild(0).GetComponent<TMP_Text>().text = string.Format("{0}", msg);
    }
}
