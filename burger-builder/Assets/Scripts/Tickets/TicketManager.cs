using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TicketManager : ManagerParent<TicketData>
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
    
    private Vector3Int globalAnchorOffset = new Vector3Int(-2, -3, 0);
     
    #region Set Up

    private void Awake()
    {
        m_UIManager = GetComponent<TicketUIManager>();
    }

    //enable called by game manager to subscribe after game manager instance has been set up
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.OnLevelEndEarly += HandleLevelEndEarly;

        DragManager.OnItemDropped += CheckCompletedTicket;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnLevelEndEarly -= HandleLevelEndEarly;
        
        DragManager.OnItemDropped -= CheckCompletedTicket;
    }

    protected override List<TicketData> GetRawData(LevelData levelData)
    {
        return levelData.GetTickets().ToList();
    }

    protected override void HandleLevelStart(LevelData levelData)
    {
        base.HandleLevelStart(levelData);
        SetValues(levelData);

        StartCoroutine(C_SpawnTickets());
        StartCoroutine(C_SetCurrentTicket());
    }
    
    protected override void SetValues(LevelData levelData)
    {
        m_levelTickets = m_levelDataItems;
        m_spawnInterval = m_baseSpawnInterval / levelData.GetDifficulty();
    }

    #endregion

    public void NewTicket()
    {
        //remove UI ticket
        m_UIManager.RemoveTicket(m_currentTicket);
        
        m_completedTickets++;
        //set new current ticket
        StartCoroutine(C_SetCurrentTicket());
    }
    
    //clear all the stored ticket data
    public override void Reset()
    {
        base.Reset();
        m_UIManager.RemoveAllTickets();
        m_ticketOrder.Clear();
        m_currentTicket = null;
        m_levelTickets = null;
        m_completedTickets = 0;
        m_levelOver = false;
    }
    
    //clear the tickets in the queue (excludes the current ticket that was already dequeued)
    // and clear the ui tickets, setting the start index at 1 to skip the current ticket ui
    public void ClearTicketQueue()
    {
        m_ticketOrder.Clear();
        m_UIManager.RemoveAllTickets(1);
    }
    
    #region Check Ticket Completed

    private void CheckCompletedTicket(TicketData currentTicket, BoardRenderer boardRenderer)
    {
        if (currentTicket == null)
            return;
        
        ItemStructure[] ticketItems = currentTicket.GetItems();
        
        if (currentTicket != null && CheckItemCount(boardRenderer, ticketItems))
        {
            if (CheckItemPositions(boardRenderer, ticketItems))
                GameManager.s_instance.TicketCleared();
        }
    }
    
    public bool CheckItemCount(BoardRenderer boardRenderer, ItemStructure[] ticketItems)
    {
        int itemCount = boardRenderer.GetComponentsInChildren<Item>().Length;
        return itemCount == ticketItems.Length;
    }
    
    public bool CheckItemPositions(BoardRenderer boardRenderer, ItemStructure[] ticketItems)
    {
        int occupiedTilesCount = boardRenderer.GetOccupiedTilesCount();
        int matches = 0;

        //go through list of items
        for (int i = 0; i < ticketItems.Length; i++)
        {
            //check if the item map contains an item at the required position
            Vector3Int ticketPos = ConvertTicketPositions(ticketItems[i].position, ticketItems[i].GetAnchorOffset());
            Item item = boardRenderer.CheckItem(ticketPos);
            
            //check if the shape of the item on the map matches the required item & required rotation
            if (ItemChecker(item, ticketItems[i]))
            {
                matches++;

                //for each cell in the item, subtract from the number of total tiles there should be
                foreach (Vector3Int cell in ticketItems[i].cells)
                {
                    occupiedTilesCount--;
                }
            }
        }

        //if the number of correct items is the number of required items
        return matches == ticketItems.Length && occupiedTilesCount == 0;
    }

    //convert from ticket ui tilemap coords into actual tilemap coords
    private Vector3Int ConvertTicketPositions(Vector3Int ticketPos, Vector3Int anchorOffset)
    {
        return ticketPos + globalAnchorOffset + anchorOffset;
    }

    private bool ItemChecker(Item boardItem, ItemStructure ticketItem)
    {
        return boardItem != null && ticketItem.shape == boardItem.GetShape() &&
               ticketItem.rotation == boardItem.GetRotation();
    }
    
    #endregion

    #region Coroutines

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
    private IEnumerator C_SetCurrentTicket()
    {
        m_currentTicket = null;
        yield return new WaitUntil(CheckQueue);

        m_currentTicket = m_ticketOrder.Dequeue();
    }
    
    #endregion
    
    private bool CheckQueue() { return m_ticketOrder.Count > 0; }
    public TicketData GetCurrentTicket() { return m_currentTicket; }
    public int GetCompletedTickets() { return m_completedTickets; }
    private void HandleLevelEndEarly() { m_levelOver = true; }
}
