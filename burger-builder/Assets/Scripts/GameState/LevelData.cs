using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [field: SerializeField] private int level;
    [Range(1, 5)]
    [field: SerializeField] private int[] difficultyRange;
    [Range(5.0f, 60.0f)]
    [field: SerializeField] private float timeLimit;
    [field: SerializeField] private int ticketRequirement;

    [SerializeField] private TicketData[] tickets;
    [SerializeField] private PerkData[] perks;
    [field: SerializeField] private ShapeData[] shapes;

    public int GetLevel() { return level; }
    public TicketData[] GetTickets() { return tickets; }
    public PerkData[] GetPerks() { return perks; }
    public ShapeData[] GetShapes() { return shapes.Distinct().ToArray(); } //remove duplicate shapes
    public float GetTimeLimit() { return timeLimit; }
    public int GetTicketRequirement() { return ticketRequirement; }
    public float GetDifficulty() { return (float)difficultyRange.Average(); }

#if UNITY_EDITOR

    private void OnValidate()
    {
        FillLevelData();
    }

    //<summary>
    //load all the tickets scriptable objects
    //then return only the tickets with the level's difficulty
    //</summary>
    private List<TicketData> LoadTickets()
    {
        List<TicketData> m_allTickets = LoadAll<TicketData>("Tickets");
        List<TicketData> difficultyTickets = new List<TicketData>();

        foreach (TicketData data in m_allTickets)
        {
            int difficulty = data.GetDifficulty();
            if (difficultyRange.Contains(difficulty))
            {
                difficultyTickets.Add(data);
            }
        }

        return difficultyTickets;
    }

    private List<T> LoadAll<T>(string folderPath) where T : UnityEngine.Object
    {
        return Utilities.LoadList<T>(folderPath);
    }

    private void FillLevelData()
    {
        tickets = LoadTickets().ToArray();
        perks = LoadAll<PerkData>("Perks").ToArray();
    }

#endif
}
