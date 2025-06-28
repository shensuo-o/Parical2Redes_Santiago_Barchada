using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform Target;
    public void SetTarget(Transform target)
    {
        Target = target;
    } 

    void Update()
    {
        transform.position = new Vector3 (Target.position.x, Target.position.y, transform.position.z);
    }
}
