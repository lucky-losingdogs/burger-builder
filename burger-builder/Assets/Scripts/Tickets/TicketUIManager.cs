using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketUIManager : MonoBehaviour
{
    [Header ("Objects")]
    [SerializeField] private GameObject m_ticketPrefab;
    [SerializeField] private Transform m_firstTicketParent;
    [SerializeField] private Transform m_ticketLineParent;
    [SerializeField] private Canvas m_canvas;

    [Header("Values")]
    [SerializeField] private float m_slideDuration = 5f;
    [SerializeField] private float m_padding = 5.0f;

    private List<GameObject> m_tickets = new List<GameObject>();

    private float m_targetDown, m_targetLeft;

    private void Awake()
    {
        if (m_ticketPrefab == null)
            Debug.LogError("Ticket prefab is null");
        if (m_ticketLineParent == null)
            Debug.LogError("Ticket parent 2 is null");
        if (m_firstTicketParent == null)
            Debug.LogError("Ticket parent 1 is null");
    }

    //spawn ticket UI and populate the UI using the data
    public void SpawnTicket(TicketData data)
    {
        //spawn it in canvas so it isn't locked into the parent's group layout immediately
        GameObject newTicket = Instantiate(m_ticketPrefab, m_ticketLineParent);

        TicketUI ui = newTicket.GetComponent<TicketUI>();
        if (ui != null)
        { 
            ui.Populate(data);
            m_tickets.Add(newTicket);
            
            if (m_tickets.Count == 1)
                SetFirstTicket();
            
            //StartCoroutine(C_AnimateTicket(newTicket, m_targetLeft));
        }
    }

    //find the first ticket in the list with the data and remove it
    public void RemoveTicket(TicketData data)
    {
        List<GameObject> tempList = m_tickets;

        foreach (GameObject ticket in tempList)
        {
            if (ticket == null)
                continue;

            TicketUI ticketUI = ticket.GetComponent<TicketUI>();
            if (ticketUI == null)
                continue;

            TicketData currentData = ticketUI.GetData();
            if (data == currentData)
            {
                Destroy(ticket);
                m_tickets.Remove(ticket);

                //set new first ticket in ui
                if (m_tickets.Count > 0)
                    SetFirstTicket();
                return;
            }
        }
    }

    public void RemoveAllTickets()
    {
        foreach (GameObject ticket in m_tickets)
        {
            if (ticket == null)
                continue;

            Destroy(ticket);
        }

        m_tickets = new List<GameObject>();
    }

    //set ticket to the main ui ticket
    private void SetFirstTicket()
    {
        GameObject currentTicket = m_tickets[0];
        TicketUI ui = currentTicket.GetComponent<TicketUI>();
        TicketData data = ui.GetData();
        if (ui != null && data != null)
        {
            ui.ToggleIcon(false);
            Vector2 ticketSize = SetParent(currentTicket);
            ui.RebuildLayout(ticketSize, m_firstTicketParent.position);
        }
    }

    private Vector2 SetParent(GameObject currentTicket)
    {
        RectTransform parentRect = m_firstTicketParent.GetComponent<RectTransform>();
        RectTransform ticketRect = currentTicket.GetComponent<RectTransform>();
        currentTicket.transform.SetParent(m_firstTicketParent);

        if (parentRect != null && ticketRect != null)
        {
            ticketRect.position = parentRect.position;
            ticketRect.sizeDelta = parentRect.sizeDelta;
        }
        return parentRect.sizeDelta;
    }
}
