using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DamageStat
{
    [FormerlySerializedAs("minDamage")]
    public float damage = 10f;

    public bool IsValid => damage >= 0f;
}
