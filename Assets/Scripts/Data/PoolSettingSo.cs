using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolSettings", menuName = "Settings/Pool Settings")]
public class PoolSettingSo : ScriptableObject
{
    [SerializeField] private PoolSetting[] poolSettings;
    public IReadOnlyList <PoolSetting> PoolSettings => poolSettings;
}