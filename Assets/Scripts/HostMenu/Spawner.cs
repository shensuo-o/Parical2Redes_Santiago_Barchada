using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] private NetworkPrefabRef _playerPrefab1;
    [SerializeField] private NetworkPrefabRef _playerPrefab2;

    private float _inputDirection;
    private bool _dashBuffered;
    private bool _shootBuffered;
    private bool _jumpBuffered;

    void Update()
    {
        _inputDirection = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _dashBuffered = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            _jumpBuffered = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            _jumpBuffered = false;
        }

        _shootBuffered = Input.GetMouseButton(0);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            runner.Spawn(_playerPrefab1, null, null, player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (LocalPlayerReference.Instance == null)
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = LocalPlayerReference.Instance.transform.position;
        Vector2 aimDir = (mouseWorld - playerPos).normalized;

        var data = new NetworkInputData
        {
            inputDir = _inputDirection,
            isDashing = _dashBuffered,
            isShoot = _shootBuffered,
            aimDirection = aimDir,
            isJumping = _jumpBuffered,
        };

        // Limpiar el buffer después de enviar
        _dashBuffered = false;

        input.Set(data);

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        runner.Shutdown();
    }


    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
