using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
/// Small data class for the weapon damage value.
public class DamageStat
{
    [FormerlySerializedAs("minDamage")]
    public float damage = 10f;

    public bool IsValid => damage >= 0f;
}
