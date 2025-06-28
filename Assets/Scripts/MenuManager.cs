using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Fusion;
using System.Threading.Tasks;

public class MenuManager : NetworkBehaviour
{
    public NetworkRunner _runner;
    public async Task FindLobby()
    {
        var result = await _runner.JoinSessionLobby(SessionLobby.Custom, "lvl1");
        if(!result.Ok)
        {
            Debug.Log("No se pudo unir");
        }
        else
        {
            Debug.Log("Se pudo unir");
        }
    }
    public void Play()
    {
        var clientTask = FindLobby();
    }
}
