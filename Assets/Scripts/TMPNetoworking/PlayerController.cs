using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Vector3 velocity;
    private CharacterController controller;
    private Vector3 lastPosition;
    private float updatePeriod = 0.1f;
    private GameObject playerObject;


    void Start()
    {
        // PoolManager에서 오브젝트 가져오기
        playerObject = PoolManager.Instance.GetObject();
        if (playerObject == null)
        {
            DebugOpt.LogError("[Error] failed to bring playerObj");
        }
        else
        {
            playerObject.transform.position = Vector3.zero;
        }
        controller = playerObject.GetComponent<CharacterController>();
        StartCoroutine(SendPlayerPositionRoutine(updatePeriod));
    }
    private void FixedUpdate()
    {
        // 없을 시 flow처리
        if (playerObject == null || controller == null)
        {
            DebugOpt.LogError("[Error] playerObject or controller is null");
            return;
        }

        // 입력 처리
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dirVec = new Vector3(horizontal, vertical, 0).normalized;
        if (dirVec.magnitude >= 0.1f)
        {
            controller.Move(dirVec * moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator SendPlayerPositionRoutine(float updatePeriod)
    {
        while (true)
        {
            if (playerObject == null) 
                yield break;
            // 현재 위치와 마지막 전송 위치 비교
            if (Vector3.Distance(playerObject.transform.position, lastPosition) > 0.1f)
            {
                lastPosition = playerObject.transform.position;
                DebugOpt.Log("[Info] SendPlayerPositionRoutine to sm");
                SocketManager.Instance.SendPlayerPosition(lastPosition);
            }
            yield return new WaitForSeconds(updatePeriod);
        }
    }

    // 코루틴 프리
    private void OnDestroy()
    {
        // PoolManager 회수
        if (playerObject != null)
        {
            PoolManager.Instance.ReturnObject(playerObject);
        }
        StopAllCoroutines();
    }
}
