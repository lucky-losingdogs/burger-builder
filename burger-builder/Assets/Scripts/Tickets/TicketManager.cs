using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TicketManager : MonoBehaviour
{
    [SerializeField] private float m_baseSpawnInterval = 2.5f;
    private float m_spawnInterval;
    private float m_intervalDecrease = 1;

    private TicketUIManager m_UIManager;
    private TicketData[] m_levelTickets;

    private Queue<TicketData> m_ticketOrder = new Queue<TicketData>();
    private TicketData m_currentTicket = null;
    
    private int m_completedTickets = 0;
    private bool m_levelOver = false;
     
    #region Set Up

    private void Awake()
    {
        m_UIManager = GetComponent<TicketUIManager>();
    }

    //enable called by game manager to subscribe after game manager instance has been set up
    public void OnEnable()
    {
        GameManager.OnLevelStart += HandleLevelStart;
        GameManager.OnLevelEndEarly += HandleLevelEndEarly;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStart -= HandleLevelStart;
        GameManager.OnLevelEndEarly -= HandleLevelEndEarly;
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
        
        m_completedTickets++;
        //set new current ticket
        StartCoroutine(SetCurrentTicket());
    }

    private void SetValues(LevelData levelData)
    {
        List<TicketData> tempTickets = levelData.GetTickets().ToList();
        tempTickets = Utilities.ExtendList(tempTickets, 50);
        m_levelTickets = Utilities.Shuffle(tempTickets).ToArray();
        
        m_spawnInterval = m_baseSpawnInterval / levelData.GetDifficulty();
    }

    //clear all the stored ticket data
    public void ResetTickets()
    {
        m_UIManager.RemoveAllTickets();
        m_ticketOrder.Clear();
        m_currentTicket = null;
        m_levelTickets = null;
        m_completedTickets = 0;
        m_levelOver = false;
    }

    //spawn ticket ui + add ticket data to queue
    private IEnumerator C_SpawnTickets()
    {
        float elapsedTime = Time.deltaTime;
        float newSpawnInterval = m_spawnInterval;

        while (!m_levelOver)
        {
            foreach (TicketData data in m_levelTickets)
            {
                m_UIManager.SpawnTicket(data);
                m_ticketOrder.Enqueue(data);
                yield return new WaitForSeconds(newSpawnInterval);

                newSpawnInterval = Mathf.Max(2, newSpawnInterval - m_intervalDecrease * elapsedTime);
            }
            
            m_levelTickets = Utilities.Shuffle(m_levelTickets.ToList()).ToArray();
        }
        
        Debug.Log("ticket spawn loop over");
    }

    //wait for the queue to have a ticket
    //then dequeue and set it as the current ticket
    private IEnumerator SetCurrentTicket()
    {
        m_currentTicket = null;
        yield return new WaitUntil(CheckQueue);

        m_currentTicket = m_ticketOrder.Dequeue();
    }

    private bool CheckQueue() { return m_ticketOrder.Count > 0; }
    public TicketData GetCurrentTicket() { return m_currentTicket; }
    public int GetCompletedTickets() { return m_completedTickets; }
    private void HandleLevelEndEarly() { m_levelOver = true; }
}
