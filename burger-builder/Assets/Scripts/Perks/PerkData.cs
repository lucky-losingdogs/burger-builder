using UnityEngine;
using Logic;

[CreateAssetMenu(fileName = "PerkData", menuName = "Scriptable Objects/PerkData")]
public class PerkData : ScriptableObject
{
    [field: SerializeField] private string name;
    [field: SerializeField] private PerkTypes perkType;
    [Range(3, 100)] [field: SerializeField] private float rank;
    [field: SerializeField] private float duration;
    
    public PerkTypes GetPerkType() => perkType;
    public string GetName() => name;
    public float GetRank() => rank;
}