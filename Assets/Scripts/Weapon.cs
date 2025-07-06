using UnityEngine;
using Fusion;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrel;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || !GetInput(out NetworkInputData input)) return;

        float angle = Mathf.Atan2(input.aimDirection.y, input.aimDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (input.isFirePressed)
        {
            Runner.Spawn(bulletPrefab, barrel.position, barrel.rotation);
        }
    }
}
