using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    [SerializeField] private float OffSetAmount;

    [SerializeField] private Image LifeBar;

    [SerializeField] private Transform TargetTransform;

    Vector3 offset;

    public void Initialize(Transform target)
    {
        TargetTransform = target;

        offset = Vector3.up * OffSetAmount;
    }

    public void UpdateFillAmount(float amount)
    {
        LifeBar.fillAmount = amount;
    }

    public void UpdatePosition()
    {
        if (!TargetTransform) return;

        transform.position = TargetTransform.position + offset;
    }
}
