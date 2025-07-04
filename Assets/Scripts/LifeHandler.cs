using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LifeHandler : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(CurrentLifeChanged))]
    private byte CurrentLife { get; set; }

    private const byte MAX_LIFE = 100;
    byte _maxDeaths = 3;

    [Networked, OnChangedRender(nameof(DeadStateChanged))]
    private NetworkBool IsDead { get; set; }

    private LifeBarItem _myLifeBar;

    public event Action<bool> OnDeadChanged = delegate { };
    public event Action OnRespawn = delegate { };

    public event Action OnLeft = delegate { };

    public override void Spawned()
    {
        _myLifeBar = LifeBarHandler.Instance.AddLifeBar(this);

        if (HasStateAuthority)
        {
            CurrentLife = MAX_LIFE;
        }
        else
        {
            CurrentLifeChanged();
        }
    }

    public void TakeDamage(byte dmg)
    {
        if (dmg > CurrentLife) dmg = CurrentLife;

        CurrentLife -= dmg;

        if (CurrentLife != 0) return;

        _maxDeaths--;

        if (_maxDeaths == 0)
        {
            DisconnectPlayer();
            return;
        }

        IsDead = true;
        StartCoroutine(Server_RespawnCooldown());
    }

    IEnumerator Server_RespawnCooldown()
    {
        yield return new WaitForSeconds(2f);

        Server_Resurrect();
    }

    void Server_Resurrect()
    {
        OnRespawn();
        IsDead = false;
        CurrentLife = MAX_LIFE;
    }

    void DeadStateChanged()
    {
        GetComponentInParent<HitboxRoot>().HitboxRootActive = !IsDead;

        OnDeadChanged(IsDead);
    }

    void CurrentLifeChanged()
    {
        _myLifeBar.UpdateLife(CurrentLife / (float)MAX_LIFE);
    }

    void DisconnectPlayer()
    {
        if (!Object.HasInputAuthority)
        {
            Runner.Disconnect(Object.InputAuthority);
        }

        Runner.Despawn(Object);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnLeft();
    }
}
