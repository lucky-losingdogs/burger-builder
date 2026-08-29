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
    [field: SerializeField] private Cutscenes cutscene;

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
    public Cutscenes GetCutscene() { return cutscene; }

#if UNITY_EDITOR

    [field: SerializeField] private bool m_loadTickets = false;
    
    private void OnValidate()
    {
        FillLevelData();
    }

    //<summary>
    //load all the tickets scriptable objects
    //then return only the tickets with the level's difficulty
    //</summary>
    private List<TicketData> GetDifficultyTickets()
    {
        List<TicketData> allTickets = LoadAll<TicketData>("Tickets");
        List<TicketData> difficultyTickets = new List<TicketData>();

        foreach (TicketData data in allTickets)
        {
            int difficulty = data.GetDifficulty();
            if (difficultyRange.Contains(difficulty))
            {
                difficultyTickets.Add(data);
            }
        }

        return difficultyTickets;
    }

    private List<TicketData> AddTickets(List<TicketData> difficultyTickets)
    {
        List<TicketData> result = tickets?.ToList() ?? new List<TicketData>();

        foreach (TicketData ticket in difficultyTickets)
        {
            if (!result.Contains(ticket))
            {
                result.Add(ticket);
            }
        }

        return result;
    }

    private List<T> LoadAll<T>(string folderPath) where T : UnityEngine.Object
    {
        return Utilities.LoadList<T>(folderPath);
    }

    private void FillLevelData()
    {
        if (m_loadTickets)
        {
            tickets = AddTickets(GetDifficultyTickets()).ToArray();
            m_loadTickets = false;
        }
        
        perks = LoadAll<PerkData>("Perks").ToArray();
    }

#endif
}
