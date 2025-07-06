using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public float movementInput;
    public NetworkBool isFirePressed;
    public NetworkButtons networkButtons;
    public Vector2 aimDirection;
}

enum MyButtons
{
    Jump = 0,
    Dash = 1,
}