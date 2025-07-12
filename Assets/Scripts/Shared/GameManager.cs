using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject _winImage;
    [SerializeField] GameObject _loseImage;

    [SerializeField] List<PlayerRef> _clients;

    [SerializeField] bool _gameStarted;
    [SerializeField] GameObject _waitingPanel;

    private void Awake()
    {
        Instance = this;

        _clients = new List<PlayerRef>();
    }

    public void AddToList(Player player)
    {
        if (!HasStateAuthority) return; // Solo el host gestiona la lista

        var playerRef = player.Object.InputAuthority;

        if (!_clients.Contains(playerRef))
            _clients.Add(playerRef);

        if (_clients.Count >= 2 && !_gameStarted)
        {
            _gameStarted = true;
            _waitingPanel.SetActive(false);
            StartGame();
        }
    }

    /*public void AddToList(Player player)
    {
        var playerRef = player.Object.StateAuthority;

        if (_clients.Contains(playerRef)) return;

        _clients.Add(playerRef);
    }*/

    void StartGame()
    {
        RPC_HideWaitingPanel(); // <- Llamamos al RPC para ocultarlo en todos

        foreach (var player in FindObjectsOfType<Player>())
        {
            player.SetCanMove(true);
        }
    }

    void RemoveFromList(PlayerRef client)
    {
        if (!HasStateAuthority) return;

        _clients.Remove(client);

        if (_clients.Count == 1)
        {
            // El último jugador restante gana
            RPC_Win(_clients[0]);
        }
    }


    [Rpc]
    public void RPC_Defeat(PlayerRef client)
    {
        if (client == Runner.LocalPlayer)
        {
            ShowDefeatImage();
        }

        RemoveFromList(client);

        if (_clients.Count == 1 && HasStateAuthority)
        {
            RPC_Win(_clients[0]);
        }
    }

    [Rpc]
    void RPC_Win([RpcTarget] PlayerRef client)
    {
        ShowWinImage();
    }

    [Rpc]
    void RPC_HideWaitingPanel()
    {
        _waitingPanel.SetActive(false);
    }

    void ShowDefeatImage()
    {
        _loseImage.SetActive(true);
    }

    void ShowWinImage()
    {
        _winImage.SetActive(true);
    }
}
