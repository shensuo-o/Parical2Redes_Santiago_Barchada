using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInputs : MonoBehaviour
{
    private NetworkInputData _networkInputData;

    private bool _isJumpPressed;
    private bool _isFirePressed;

    void Start()
    {
        _networkInputData = new NetworkInputData();
    }

    void Update()
    {
        _networkInputData.movementInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isFirePressed = true;
        }

        //_isFirePressed |= Input.GetKeyDown(KeyCode.Space);

        _isJumpPressed |= Input.GetKeyDown(KeyCode.W);
    }

    public NetworkInputData GetLocalInputs()
    {
        _networkInputData.isFirePressed = _isFirePressed;
        _isFirePressed = false;

        _networkInputData.networkButtons.Set(MyButtons.Jump, _isJumpPressed);
        _isJumpPressed = false;

        return _networkInputData;
    }
}
