using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketManager : MonoBehaviour
{
    private float m_spawnInterval;
    private float m_intervalDecrease;

    private TicketUIManager m_UIManager;
    private TicketData[] m_levelTickets;
    private int m_ticketIndex = 0;

    private Queue<TicketData> m_ticketOrder = new Queue<TicketData>();
    private TicketData m_currentTicket = null;
     
    #region Set Up

    private void Awake()
    {
        m_UIManager = GetComponent<TicketUIManager>();
    }

    //enable called by game manager to subscribe after game manager instance has been set up
    public void OnEnable()
    {
        GameManager.OnLevelStart += HandleLevelStart;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStart -= HandleLevelStart;
    }

    #endregion

    private void HandleLevelStart(LevelData levelData)
    {
        SetValues(levelData);

        StartCoroutine(C_SpawnTickets());
        StartCoroutine(SetCurrentTicket());
    }

    public void NewTicket()
    {
        //remove UI ticket
        m_UIManager.RemoveTicket(m_currentTicket);
        //set new current ticket
        StartCoroutine(SetCurrentTicket());
    }

    private void SetValues(LevelData levelData)
    {
        m_levelTickets = levelData.GetTickets();

        m_spawnInterval = levelData.GetTimeLimit() / m_levelTickets.Length;
        m_intervalDecrease = m_levelTickets.Length;
    }

    //clear all the stored ticket data
    public void ResetTickets()
    {
        m_UIManager.RemoveAllTickets();
        m_ticketOrder.Clear();
        m_currentTicket = null;
        m_levelTickets = null;
    }

    //spawn ticket ui + add ticket data to queue
    private IEnumerator C_SpawnTickets()
    {
        float elapsedTime = Time.deltaTime;
        float newSpawnInterval = m_spawnInterval;

        foreach (TicketData data in m_levelTickets)
        {
            m_UIManager.SpawnTicket(data);
            m_ticketOrder.Enqueue(data);
            yield return new WaitForSeconds(newSpawnInterval);

            newSpawnInterval = Mathf.Max(2, newSpawnInterval - m_intervalDecrease * elapsedTime);
        }
    }

    //wait for the queue to have a ticket
    //then dequeue and set it as the current ticket
    private IEnumerator SetCurrentTicket()
    {
        if (m_levelTickets.Length == m_ticketIndex)
        { 
            GameUIManager.s_instance.ToggleWinMenu(true);
            yield break;
        }

        m_currentTicket = null;
        yield return new WaitUntil(CheckQueue);

        m_currentTicket = m_ticketOrder.Dequeue();
        m_ticketIndex++;
    }

    private bool CheckQueue() { return m_ticketOrder.Count > 0; }

    public TicketData GetCurrentTicket() { return m_currentTicket; }
}
