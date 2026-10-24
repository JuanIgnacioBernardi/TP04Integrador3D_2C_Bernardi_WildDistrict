using System;
using UnityEngine;

[Serializable]
public class PoolSetting
{
    [SerializeField] private GameObject prefab;
    [SerializeField, Min(1)] private int quantity = 10;
    [SerializeField] private bool canGrow = true;

    public GameObject Prefab => prefab;
    public int Quantity => quantity;
    public bool CanGrow => canGrow;
}