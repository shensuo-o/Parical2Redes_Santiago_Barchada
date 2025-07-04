using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LifeBarHandler : MonoBehaviour
{
    public static LifeBarHandler Instance { get; private set; }

    [SerializeField] private LifeBarItem lifeBarItemPrefab;

    private List<LifeBarItem> _lifeBarsList;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _lifeBarsList = new List<LifeBarItem>();
    }

    public LifeBarItem AddLifeBar(LifeHandler owner)
    {
        var newLifeBar = Instantiate(lifeBarItemPrefab, transform)
            .SetOwner(owner);

        _lifeBarsList.Add(newLifeBar);

        owner.OnLeft += () =>
        {
            _lifeBarsList.Remove(newLifeBar);

            Destroy(newLifeBar.gameObject);
        };

        return newLifeBar;
    }

    void LateUpdate()
    {
        foreach (var lifeBarItem in _lifeBarsList)
        {
            lifeBarItem.UpdatePosition();
        }
    }
}
