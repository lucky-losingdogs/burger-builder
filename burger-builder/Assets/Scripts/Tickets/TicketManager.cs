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
    private TicketData m_previousTicket = null;
    
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
        LevelManager.OnLevelEndEarly += HandleLevelEndEarly;
        LevelManager.OnTicketCleared += NewTicket;
        LevelManager.OnLevelEnd += Reset;

        DragManager.OnItemDropped += CheckCompletedTicket;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelManager.OnLevelEndEarly -= HandleLevelEndEarly;
        LevelManager.OnTicketCleared -= NewTicket;
        LevelManager.OnLevelEnd -= Reset;
        
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

    private void NewTicket()
    {
        m_previousTicket = m_currentTicket;
        
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
        m_UIManager.Reset();
        m_ticketOrder.Clear();
        m_currentTicket = null;
        m_previousTicket = null;
        m_levelTickets = null;
        m_completedTickets = 0;
        m_levelOver = false;
    }
    
    #region Perk Behaviour
    
    //clear the tickets in the queue (excludes the current ticket that was already dequeued)
    // and clear the ui tickets, setting the start index at 1 to skip the current ticket ui
    public void ClearTicketQueue()
    {
        //get number of tickets in the queue that are going to be cleared
        int clearedTickets = m_ticketOrder.Count;
        
        m_ticketOrder.Clear();
        m_UIManager.RemoveAllTickets(1);

        MultipleTicketsCleared(clearedTickets);
    }

    public void ClearByTicketType(TicketData ticketToClear)
    {
        int clearedTickets = 0;
        Queue<TicketData> newQueue = new Queue<TicketData>();

        foreach (TicketData ticket in m_ticketOrder)
        {
            if (ticket == ticketToClear)
            {
                clearedTickets++;
                m_UIManager.RemoveTicket(ticket);
            }
            else
            {
                newQueue.Enqueue(ticket);
            }
        }

        m_ticketOrder = newQueue;
        
        MultipleTicketsCleared(clearedTickets);
    }

    private void MultipleTicketsCleared(int clearedTickets)
    {
        //increase completed ticket count
        m_completedTickets += clearedTickets;
        
        //update combo and ticket req ui
        LevelManager.s_instance.HandleTicketsCleared(clearedTickets);
        
        //ensure the current ticket is set again after clearing queue
        StartCoroutine(C_SetCurrentTicket());
    }
    
    #endregion
    
    #region Check Ticket Completed

    private void CheckCompletedTicket(TicketData currentTicket, BoardRenderer boardRenderer)
    {
        if (currentTicket == null)
            return;
        
        ItemStructure[] ticketItems = currentTicket.GetItems();
        
        if (CheckItemCount(boardRenderer, ticketItems) && CheckItemPositions(boardRenderer, ticketItems))
        {
            LevelManager.s_instance.HandleTicketCleared();
        }
    }
    
    public bool CheckItemCount(BoardRenderer boardRenderer, ItemStructure[] ticketItems)
    {
        int itemCount = boardRenderer.GetComponentsInChildren<Item>().Length;
        return itemCount == ticketItems.Length;
    }
    
    public bool CheckItemPositions(BoardRenderer boardRenderer, ItemStructure[] ticketItems)
    {
        int totalUnmatchedTiles = boardRenderer.GetOccupiedTilesCount();
        int matches = 0;
    
        //go through list of items
        foreach (ItemStructure ticketItem in ticketItems)
        {
            //check if the item map contains an item at the required position
            Vector3Int ticketPos = ConvertTicketPositionToBoard(ticketItem.position, ticketItem.GetAnchorOffset());
            Item item = boardRenderer.CheckItem(ticketPos);
            
            //check if the shape of the item on the map matches the required item & required rotation
            if (ItemChecker(item, ticketItem))
            {
                //get cells for the ticket item, convert to board coordinates
                Vector3Int[] ticketCells = ticketItem.GetCells().Select(cell => ConvertTicketCellToBoard(cell,ticketItem.position)).ToArray();
                HashSet<Vector3Int> itemCells = item.GetCells().Select(cell => cell + item.GetPosition()).ToHashSet();
                
                matches++;
    
                //check if each cell is in the correct position
                //for each cell in the item, subtract from the number of total tiles there should be
                foreach (Vector3Int ticketCell in ticketCells)
                {
                    if (itemCells.Contains(ticketCell))
                        totalUnmatchedTiles--;
                }
            }
        }
    
        //if the number of correct items is the number of required items
        return matches == ticketItems.Length && totalUnmatchedTiles == 0;
    }

    //convert from ticket ui tilemap coords into actual tilemap coords
    private Vector3Int ConvertTicketPositionToBoard(Vector3Int ticketPos, Vector3Int anchorOffset)
    {
        return ticketPos + globalAnchorOffset + anchorOffset;
    }
    
    //convert cells from relative positions to actual tilemap coords based on the ticket pos + global offset
    private Vector3Int ConvertTicketCellToBoard(Vector3Int ticketCell, Vector3Int ticketPos)
    {
        return ticketCell + globalAnchorOffset + ticketPos;
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
            
            m_levelTickets = Utilities.Shuffle(m_levelTickets.ToList()).ToArray();;
        }
        
        Debug.Log("ticket spawn loop over");
    }

    //wait for the queue to have a ticket
    //then dequeue and set it as the current ticket
    private IEnumerator C_SetCurrentTicket()
    {
        m_currentTicket = null;
        
        yield return new WaitUntil(() => m_ticketOrder.Count > 0);

        if (m_ticketOrder.Count > 0)
            m_currentTicket = m_ticketOrder.Dequeue();
    }
    
    #endregion
    
    public TicketData GetCurrentTicket() => m_currentTicket;
    public int GetCompletedTickets() => m_completedTickets;
    public TicketData GetPreviousCurrentTicket() => m_previousTicket;
    private void HandleLevelEndEarly() => m_levelOver = true;
}
