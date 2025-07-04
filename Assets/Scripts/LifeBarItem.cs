using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBarItem : MonoBehaviour
{
    private Transform _owner;

    private const float HEAD_OFFSET = 1.5F;

    [SerializeField] private Image _myLifeImage;

    public LifeBarItem SetOwner(LifeHandler owner)
    {
        _owner = owner.transform;

        return this;
    }

    public void UpdateLife(float val)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateLifeOnTime(val));
    }

    IEnumerator UpdateLifeOnTime(float val)
    {
        var ticks = 0f;

        var startAmount = _myLifeImage.fillAmount;

        while (ticks < 1)
        {
            ticks += Time.deltaTime * 2;

            _myLifeImage.fillAmount = Mathf.Lerp(startAmount, val, ticks);

            yield return null;
        }
    }

    public void UpdatePosition()
    {
        transform.position = _owner.position + Vector3.up * HEAD_OFFSET;
    }
}
