using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float LifeTime;
    [SerializeField] private float Damage;
    [SerializeField] private int Layer;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        LifeTime -= Runner.DeltaTime;
        if (LifeTime <= 0)
        {
            Runner.Despawn(Object);
        }
        transform.position += transform.right * Speed * Runner.DeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == Layer)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.RPC_TakeDamage(Damage);
            }
            //collision.gameObject.GetComponent<Player>().RPC_TakeDamage(Damage);
            Runner.Despawn(this.Object);
        }
    }
}
