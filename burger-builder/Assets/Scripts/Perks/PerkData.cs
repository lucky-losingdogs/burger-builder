using UnityEngine;
using Logic;

[CreateAssetMenu(fileName = "PerkData", menuName = "Scriptable Objects/PerkData")]
public class PerkData : ScriptableObject
{
    [field: SerializeField] private string name;
    [field: SerializeField] private PerkTypes perkType;
    [field: SerializeField] private float duration;
    
    public PerkTypes GetPerkType() { return perkType; }
    public string GetName() { return name; }

    //trigger the effect of the perk by getting the class w/ logic from data dictionary
    public void Effect()
    {
        Data.Perks.TryGetValue(perkType, out PerkLogic logic);
        logic.Effect();
    }
}