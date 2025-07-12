using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform LHand;
    [SerializeField] private Transform RHand;

    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform barrel;
    [SerializeField] private bool Fire;

    [SerializeField] private Vector2 Dir;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire = true;
        }

        Dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData data)) return;

        //Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;  

        if (Fire)
        {
            NetworkObject bullet = Runner.Spawn(bulletPref, barrel.transform.position, barrel.transform.rotation);
            Fire = false;
        }

        if (player.HorizontalInput == 1)
        {
            transform.position = RHand.transform.position;
        }
        else if(player.HorizontalInput == -1)
        {
            transform.position = LHand.transform.position;
        }
    }
}
