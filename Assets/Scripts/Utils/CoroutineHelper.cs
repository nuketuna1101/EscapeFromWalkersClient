using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelper
{
    /// <summary>
    /// 코루틴 관리 관련
    /// </summary>
    public static void RestartCor(MonoBehaviour monoBehaviour, ref Coroutine coroutine, IEnumerator routine)
    {
        if (coroutine != null)
        {
            monoBehaviour.StopCoroutine(coroutine);
        }
        coroutine = monoBehaviour.StartCoroutine(routine);
    }
    public static void StopCor(MonoBehaviour monoBehaviour, ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            monoBehaviour.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
