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

    private void Awake()
    {
        Instance = this;

        _clients = new List<PlayerRef>();
    }

    public void AddToList(Player player)
    {
        var playerRef = player.Object.StateAuthority;

        if (_clients.Contains(playerRef)) return;

        _clients.Add(playerRef);
    }

    void RemoveFromList(PlayerRef client)
    {
        _clients.Remove(client);
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

    void ShowDefeatImage()
    {
        _loseImage.SetActive(true);
    }

    void ShowWinImage()
    {
        _winImage.SetActive(true);
    }
}
