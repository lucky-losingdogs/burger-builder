using System;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PerkData", menuName = "Scriptable Objects/PerkData")]
public class PerkData : ScriptableObject
{
    [field: SerializeField] private string name;
    [field: SerializeField] private PerkTypes perkType;
    [Range(3, 100)] [field: SerializeField] private float rank;
    [field: SerializeField] private float duration;
    [field: SerializeField] private float value;
    
    public PerkTypes GetPerkType() => perkType;
    public string GetName() => name;
    public float GetRank() => rank;
    public float GetDuration() => duration;
    public float GetValue() => value;
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (duration < 0)
            duration = 0;
    }

#endif
}