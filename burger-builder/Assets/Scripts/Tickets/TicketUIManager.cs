using UnityEngine;

public class TicketUIManager : MonoBehaviour
{
    [SerializeField] private GameObject m_ticketPrefab;
    [SerializeField] private Transform m_ticketParent;

    private Canvas m_canvas;

    private void Awake()
    {
        if (m_ticketPrefab == null)
            Debug.LogError("Ticket prefab is null");
        if (m_ticketParent == null)
            Debug.LogError("Ticket parent is null");

        m_canvas = FindFirstObjectByType<Canvas>();
    }

    public void SpawnTicket(TicketData data)
    {
        GameObject newTicket = Instantiate(m_ticketPrefab, m_canvas.transform);
        TicketUI ui = newTicket.GetComponent<TicketUI>();
        if (ui != null)
        { 
            ui.Populate(data.GetSprite(), m_ticketParent);
        }
    }
}
