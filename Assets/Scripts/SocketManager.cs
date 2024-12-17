using System.Collections;
using UnityEngine;
using SocketIO;
using Quobject.SocketIoClientDotNet.Client;
using SocketIOClient;
using System.Threading.Tasks;

public class SocketManager : MonoBehaviour
{
    private SocketIOClient.SocketIO socket;
    private const string serverURL = "http://localhost:3010";

    void Start()
    {
        // SocketIO 인스턴스를 생성하고 서버와 연결
        socket = new SocketIOClient.SocketIO(serverURL);

        // 연결이 성공하면 호출되는 이벤트 핸들러
        socket.OnConnected += async (sender, e) =>
        {
            Debug.Log("[SocketIOClient] Connected to Server");
            await PingTest();
        };

        // 'pong' 이벤트 리스닝
        socket.On("pong", response =>
        {
            Debug.Log("[Pong] pong received");
        });
        // 연결 시작
        socket.ConnectAsync();
    }

    // 핑 테스트를 위한 함수
    async private Task PingTest()
    {
        // 서버에 'ping' 이벤트를 보냄
        await socket.EmitAsync("ping", "ping data");
        Debug.Log("[Ping] Ping test");
    }

    void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 서버 연결 종료
        socket.DisconnectAsync();
    }
}