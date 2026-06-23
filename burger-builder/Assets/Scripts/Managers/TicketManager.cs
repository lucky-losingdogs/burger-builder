using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO
//shorten spawn interval over time
//ratio of total tickets


public class TicketManager : MonoBehaviour
{
    [SerializeField] private float m_spawnInterval = 10.0f;
    [SerializeField] private float m_intervalDecrease = 0.3f;

    private TicketUIManager m_UIManager;
    private TicketData[] m_levelTickets;

    private Queue<TicketData> m_ticketOrder = new Queue<TicketData>();
    private static TicketData m_currentTicket = null;

    #region Set Up

    private void Awake()
    {
        m_UIManager = GetComponent<TicketUIManager>();
    }

    public void Enable()
    {
        GameManager.s_instance.OnLevelStart += HandleLevelStart;
    }

    private void OnDisable()
    {
        GameManager.s_instance.OnLevelStart -= HandleLevelStart;
    }

    #endregion

    private void HandleLevelStart(LevelData levelData)
    {
        m_levelTickets = levelData.GetTickets();

        StartCoroutine(C_SpawnTickets());
        StartCoroutine(C_SetCurrentTicket());
    }

    public void NewTicket()
    {
        //remove UI ticket
        m_UIManager.RemoveTicket();
        //set new current ticket
        StartCoroutine(C_SetCurrentTicket());
    }

    private void ChangeInterval()
    {
        //float newInternal = Mathf.Max(2.0, m_spawnInterval - m_intervalDecrease * elapsed time)
    }

    private IEnumerator C_SpawnTickets()
    {
        foreach (TicketData data in m_levelTickets)
        {
            m_UIManager.SpawnTicket(data);
            m_ticketOrder.Enqueue(data);
            yield return new WaitForSeconds(m_spawnInterval);
        }
    }

    //wait for the queue to have a ticket
    //then dequeue and set it as the current ticket
    private IEnumerator C_SetCurrentTicket()
    {
        yield return new WaitUntil(CheckTicketQueue);
        m_currentTicket = m_ticketOrder.Dequeue();
    }

    private bool CheckTicketQueue() { return m_ticketOrder.Count > 0; }

    public static TicketData GetCurrentTicket() { return m_currentTicket; }

    public void Reset()
    {
        m_ticketOrder = new Queue<TicketData>();
        m_currentTicket = null;
    }
}
