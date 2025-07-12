using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerReference : NetworkBehaviour
{
    public static LocalPlayerReference Instance { get; private set; }

    public override void Spawned()
    {
        if (!HasInputAuthority) return;

        Instance = this;
    }
}