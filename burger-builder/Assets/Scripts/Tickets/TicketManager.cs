using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//TODO
//randomise tickets
//choose fronm difficulty range of tickets
//shorten spawn interval over time + with harder lvls
//time limit per ticket
//fail state when enough tickets have expired

public class TicketManager : MonoBehaviour
{
    [SerializeField] private float m_spawnInterval = 10.0f;

    private TicketUIManager m_UIManager;
    private List<TicketData> m_allTickets = new List<TicketData>();

    private Queue<TicketData> m_ticketOrder = new Queue<TicketData>();
    private static TicketData m_currentTicket = null;

    private void Awake()
    {
        m_allTickets = Utilities.LoadList<TicketData>("Tickets");
        m_UIManager = GetComponent<TicketUIManager>();
    }

    private void Start()
    {
        StartCoroutine(C_SpawnTickets());
    }

    private IEnumerator C_SpawnTickets()
    {
        foreach (TicketData data in m_allTickets)
        {
            m_UIManager.SpawnTicket(data);
            m_ticketOrder.Enqueue(data);
            yield return new WaitForSeconds(m_spawnInterval);
        }
    }

    //private IEnumerator C_SetCurrentTicket()
    //{


    //    m_currentTicket = m_ticketOrder.Dequeue();
    //}

    public static TicketData GetCurrentTicket() { return m_currentTicket; }
}
