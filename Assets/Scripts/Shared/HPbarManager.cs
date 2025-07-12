using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HPbarManager : MonoBehaviour
{
    public static HPbarManager Instance { get; private set; }

    [SerializeField] HPbar barPrefab;

    List<HPbar> _lifeBarsInUse;

    private void Awake()
    {
        Instance = this;

        _lifeBarsInUse = new List<HPbar>();
    }

    public void CreateLifeBar(Player target)
    {
        var newBar = Instantiate(barPrefab, transform);

        newBar.Initialize(target.transform);

        _lifeBarsInUse.Add(newBar);

        target.OnLifeUpdate += newBar.UpdateFillAmount;

        target.OnDespawn += () =>
        {
            _lifeBarsInUse.Remove(newBar);
            Destroy(newBar.gameObject);
        };
    }

    private void LateUpdate()
    {
        foreach (var lifebar in _lifeBarsInUse)
        {
            lifebar.UpdatePosition();
        }
    }
}
