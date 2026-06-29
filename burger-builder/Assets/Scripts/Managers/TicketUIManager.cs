using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketUIManager : MonoBehaviour
{
    [Header ("Objects")]
    [SerializeField] private GameObject m_ticketPrefab;
    [SerializeField] private Transform m_ticketParent;
    [SerializeField] private Transform m_ticketSpawn;
    [SerializeField] private RectTransform m_ticketParentRect;
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
        if (m_ticketParent == null)
            Debug.LogError("Ticket parent is null");

        m_targetLeft = m_ticketParentRect.anchoredPosition.x - (m_ticketParentRect.rect.width * 0.5f);
    }

    //spawn ticket UI and populate the UI using the data
    public void SpawnTicket(TicketData data)
    {
        //spawn it in canvas so it isnt locked into the parent's group layout immediately
        GameObject newTicket = Instantiate(m_ticketPrefab, m_ticketSpawn);
        TicketUI ui = newTicket.GetComponent<TicketUI>();
        if (ui != null)
        { 
            ui.Populate(data, m_ticketParent);
            m_tickets.Add(newTicket);
            StartCoroutine(C_AnimateTicket(newTicket, m_targetLeft));
        }
    }

    //find the first ticket in the list with the data and remove it
    public void RemoveTicket(TicketData data)
    {
        foreach (GameObject ticket in m_tickets)
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

    public int GetTicketCount()
    {
        Transform[] allChildren = m_ticketParent.GetComponentsInChildren<Transform>();
        return allChildren.Length - 1;
    }

    private float RecalulcateTarget(RectTransform ticketRect, float targetX)
    {
        float ticketWidth = ticketRect.rect.width;

        int ticketCount = GetTicketCount();
        return targetX + (ticketCount * ticketWidth);
    }

    //calc destination/distance using num of present tickets
    private IEnumerator C_AnimateTicket(GameObject ticket, float targetX)
    {
        RectTransform ticketRect = ticket.GetComponent<RectTransform>();

        Vector2 startPos = ticketRect.anchoredPosition;
        Vector2 targetPos = new Vector2(RecalulcateTarget(ticketRect, targetX), startPos.y);
        float elapsedTime = 0f;

        //lerp x position over time
        while (elapsedTime < m_slideDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            ticketRect.anchoredPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / m_slideDuration);
            yield return null;
        }

        ticketRect.anchoredPosition = targetPos;
        ticket.transform.SetParent(m_ticketParent);
    }
}
