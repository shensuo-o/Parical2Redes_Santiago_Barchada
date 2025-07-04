using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterControllerCustom))]
[RequireComponent(typeof(WeaponHandler))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterControllerCustom _characterMovement;
    private WeaponHandler _weaponHandler;

    public override void Spawned()
    {
        _characterMovement = GetComponent<NetworkCharacterControllerCustom>();
        _weaponHandler = GetComponent<WeaponHandler>();

        if (!TryGetBehaviour(out LifeHandler lifeHandler)) return;

        lifeHandler.OnDeadChanged += b =>
        {
            enabled = !b;
        };

        lifeHandler.OnRespawn += () =>
        {
            _characterMovement.Teleport(transform.position + Vector3.up * 3);
        };
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputs)) return;

        //Movimiento
        Vector3 moveDirection = Vector3.forward * inputs.movementInput;
        _characterMovement.Move(moveDirection);

        //Salto
        if (inputs.networkButtons.IsSet(MyButtons.Jump))
        {
            _characterMovement.Jump();
        }

        //Disparo
        if (inputs.isFirePressed)
        {
            _weaponHandler.Fire();
        }
    }
}