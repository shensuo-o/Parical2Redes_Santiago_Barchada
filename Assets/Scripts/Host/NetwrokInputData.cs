using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public bool isDashing;

    public bool isShoot;

    public float inputDir;

    public bool isJumping;

    public Vector2 aimDirection;
}
