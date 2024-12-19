using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Vector3 lastPosition;
    private float updatePeriod = 0.1f;
    private GameObject playerObject;

    [Header("Grid Matcher")]
    // 격자 이동용 변수
    private bool isMoving = false;
    private const float gridSize = 1.0f;
    private Vector3 targetPosition;


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
        StartCoroutine(SendPlayerPositionRoutine(updatePeriod));
    }

    private void FixedUpdate()
    {
        // 없을 시 flow 처리
        if (playerObject == null)
        {
            Debug.LogError("[Error] playerObject is null");
            return;
        }

        // 이동 중이면 입력을 무시
        if (isMoving)
            return;

        // 입력 처리
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
        {
            // 이동 방향 설정
            Vector3 direction;
            if (Mathf.Abs(vertical) >= Mathf.Abs(horizontal))
                direction = new Vector3(0, vertical, 0).normalized;
            else
                direction = new Vector3(horizontal, 0, 0).normalized;
            targetPosition = playerObject.transform.position + direction * gridSize;
            StartCoroutine(MoveToTargetPosition());
        }
    }

    private IEnumerator MoveToTargetPosition()
    {
        isMoving = true;

        while (Vector3.Distance(playerObject.transform.position, targetPosition) > 0.01f)
        {
            // 목표 위치로 부드럽게 이동
            playerObject.transform.position = Vector3.MoveTowards(
                playerObject.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        playerObject.transform.position = targetPosition;
        isMoving = false;

        Debug.Log($"Player moved to {targetPosition}");
    }

    // 서버와 위치 동기화위해 pos 전송
    private IEnumerator SendPlayerPositionRoutine(float updatePeriod)
    {
        while (true)
        {
            if (playerObject == null) 
                yield break;
            // 현재 위치와 마지막 전송 위치 비교
            if (Vector3.Distance(playerObject.transform.position, lastPosition) > 0.1f)
            {
                //DebugOpt.Log("[Info] SendPlayerPositionRoutine to sm");
                lastPosition = playerObject.transform.position;
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
