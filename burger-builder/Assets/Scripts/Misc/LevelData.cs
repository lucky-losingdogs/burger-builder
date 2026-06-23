using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [Range(0, 10)]
    [field: SerializeField] private int[] difficultyRange;
    [field: SerializeField] private float timeLimit;
    [field: SerializeField] private int maxTicketTotal; // the number of total tickets used in the level

    [SerializeField] private TicketData[] tickets;

    public TicketData[] GetTickets() { return tickets; }

#if UNITY_EDITOR

    private void OnValidate()
    {
        UpdateTickets();
    }

    //<summary>
    //load all the tickets scriptable objects
    //then return only the tickets with the level's difficulty
    //</summary>
    private List<TicketData> LoadTickets()
    {
        List<TicketData> m_allTickets = Utilities.LoadList<TicketData>("Tickets");
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

    //<summary>
    //update the tickets used in the level based on the difficulty range, max unique tickets and max ticket count
    //</summary>
    private void UpdateTickets()
    {
        List<TicketData> tempTickets = LoadTickets();

        //if the number of ticket scriptable obj of that difficulty
        //is less than the required amount of tickets in the level,
        //fill the rest of the list with the same tickets until it reaches ticket total
        if (tempTickets.Count < maxTicketTotal)
        {
            tempTickets = Utilities.ExtendList(tempTickets, maxTicketTotal);
            tempTickets = Utilities.Shuffle(tempTickets);
        }
        //if the number of tickets is greater than the maximum total,
        //shuffle the list order then trim the excess tickets
        else
        {
            tempTickets = Utilities.Shuffle(tempTickets);
            tempTickets = Utilities.TrimExcess(tempTickets, maxTicketTotal);
        }

        tickets = tempTickets.ToArray();
    }

#endif
}
