using System.Collections.Generic;
using UnityEngine;

public class NicknameHandler : MonoBehaviour
{
    public static NicknameHandler Instance { get; private set; }

    [SerializeField] private NicknameItem _nicknameItemPrefab;

    private List<NicknameItem> _nicknames;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _nicknames = new List<NicknameItem>();
    }

    public NicknameItem AddNickname(Player owner)
    {
        var newNickname = Instantiate(_nicknameItemPrefab, transform)
                            .SetOwner(owner);

        _nicknames.Add(newNickname);

        owner.OnDespawn += () =>
        {
            _nicknames.Remove(newNickname);
            Destroy(newNickname.gameObject);
        };

        return newNickname;
    }

    void LateUpdate()
    {
        foreach (var nicknameItem in _nicknames)
        {
            nicknameItem.UpdatePosition();
        }
    }
}