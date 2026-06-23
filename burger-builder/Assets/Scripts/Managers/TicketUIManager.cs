using UnityEngine;
using System.Collections.Generic;

public class TicketUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_ticketPrefab;
    [SerializeField] private Transform m_ticketParent;

    private List<GameObject> m_tickets = new List<GameObject>();
    private Canvas m_canvas;

    private void Awake()
    {
        if (m_ticketPrefab == null)
            Debug.LogError("Ticket prefab is null");
        if (m_ticketParent == null)
            Debug.LogError("Ticket parent is null");

        m_canvas = FindFirstObjectByType<Canvas>();
    }

    //spawn ticket UI and populate the UI using the data
    public void SpawnTicket(TicketData data)
    {
        //spawn it in canvas so it isnt locked into the parent's group layout immediately
        GameObject newTicket = Instantiate(m_ticketPrefab, m_canvas.transform);
        TicketUI ui = newTicket.GetComponent<TicketUI>();
        if (ui != null)
        { 
            ui.Populate(data, m_ticketParent);
            m_tickets.Add(newTicket);
        }
    }

    //remove the first ticket in the list
    public void RemoveTicket()
    {
        Destroy(m_tickets[0]);
        m_tickets.RemoveAt(0);
    }

    //find the first ticket in the list with the data and remove it
    public void RemoveTicket(TicketData data)
    {
        foreach (GameObject ticket in m_tickets)
        {
            TicketUI ticketUI = ticket.GetComponent<TicketUI>();
            if (ticketUI == null)
                continue;

            TicketData currentData = ticketUI.GetData();
            if (data == currentData)
            {
                Destroy(ticket);
                m_tickets.Remove(ticket);
                return;
            }
        }
    }
}
