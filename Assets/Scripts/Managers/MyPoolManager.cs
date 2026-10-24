using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MyPoolManager : MonoBehaviour
{
    private sealed class PoolData
    {
        public GameObject Prefab;
        public Transform Parent;
        public bool CanGrow;
        public readonly List<IPooleable> Items = new();
    }
    private readonly Dictionary<Type, PoolData> _pools = new();
    public void Init(PoolSettingSo settings)
    {
        if (settings == null)
            return;

        foreach (PoolSetting setting in settings.PoolSettings)
            CreatePool(setting);

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
    public T GetFromPool<T>() where T : MonoBehaviour, IPooleable
    {
        if (!_pools.TryGetValue(typeof(T), out PoolData data))
            return null;

        foreach (IPooleable item in data.Items)
        {
            if (item.IsActive) continue;
            item.Activate();
            return (T)item;
        }

        if (!data.CanGrow) return null;

        IPooleable created = CreateItem(data);
        data.Items.Add(created);
        created.Activate();
        return (T)created;
    }
    public void DeactivateAll<T>() where T : MonoBehaviour, IPooleable
    {
        if (!_pools.TryGetValue(typeof(T), out PoolData data)) return;

        foreach (IPooleable item in data.Items)
            item.DeActivate();
    }
    private void CreatePool(PoolSetting setting)
    {
        if (setting.Prefab == null)
            return;

        IPooleable prototype = setting.Prefab.GetComponent<IPooleable>();
        if (prototype == null)
            return;

        Type type = prototype.GetType();
        if (_pools.ContainsKey(type))
            return;

        Transform parent = new GameObject($"Pool_{setting.Prefab.name}").transform;
        parent.SetParent(transform);

        PoolData data = new()
        {
            Prefab = setting.Prefab,
            Parent = parent,
            CanGrow = setting.CanGrow
        };
        _pools[type] = data;

        for (int i = 0; i < setting.Quantity; i++)
            data.Items.Add(CreateItem(data));
    }
    private static IPooleable CreateItem(PoolData data)
    {
        GameObject go = Instantiate(data.Prefab, data.Parent);
        IPooleable item = go.GetComponent<IPooleable>();
        item.DeActivate();
        return item;
    }
    private void OnActiveSceneChanged(Scene previous, Scene next)
    {
        foreach (PoolData data in _pools.Values)
            foreach (IPooleable item in data.Items)
                item.DeActivate();
    }
}