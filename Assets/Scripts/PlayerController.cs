using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private Vector3 velocity;
    private CharacterController controller;
    private Vector3 lastPosition;

    void Start()
    {
        // CharacterController 컴포넌트 가져오기
        controller = GetComponent<CharacterController>();
    }
    private void FixedUpdate()
    {
        // 입력 처리
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        // 이동 처리
        Vector3 move = new Vector3(horizontal, vertical, 0).normalized;
        if (move.magnitude >= 0.1f)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
        }

        // 플레이어 위치 서버 전송
        if (Vector3.Distance(transform.position, lastPosition) > 0.1f)
        {
            lastPosition = transform.position;
            SocketManager.Instance.SendPlayerPosition(transform.position);
        }
    }
}
