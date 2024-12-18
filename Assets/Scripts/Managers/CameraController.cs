using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    private Transform targetTransform;
    private Coroutine cameraTrackingCoroutine;

    public void InitCameraControl()
    {
        targetTransform = BattleManager.Instance.Players[0].transform;
        CoroutineHelper.RestartCor(this, ref cameraTrackingCoroutine, CameraTrackingRoutine());
    }

    private IEnumerator CameraTrackingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            if (transform.position.x == targetTransform.position.x 
                && transform.position.y == targetTransform.position.y) continue;

            transform.position = new Vector3(targetTransform.position.x, targetTransform.position.y, -10f);
        }
    }
}
