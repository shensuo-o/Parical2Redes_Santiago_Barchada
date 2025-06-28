using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerJoins : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private int players;
    [SerializeField] private Transform[] spawnPoints;

    private bool _initialized;
    private int _index;


    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("joined player");

        players = Runner.SessionInfo.PlayerCount;

        if (_initialized && players >= 2)
        {
            CreatePlayer1(_index);
            return;
        }

        if (player == Runner.LocalPlayer)
        {
            if (players < 2)
            {
                if (!_initialized)
                {
                    _initialized = true;
                    _index = players - 1;
                }
            }
            else
            {
                CreatePlayer2(players -1);
            }
        }
    }

    void CreatePlayer1(int playerIndex)
    {
        _initialized = false;

        var spawnPoint = spawnPoints[playerIndex];
        
            //Spawnear el personaje
            Runner.Spawn(player1Prefab, spawnPoint.position, Quaternion.identity);
    }

    void CreatePlayer2(int playerIndex)
    {
        _initialized = false;

        var spawnPoint = spawnPoints[playerIndex];

        //Spawnear el personaje
        Runner.Spawn(player2Prefab, spawnPoint.position, Quaternion.identity);
    }
}
