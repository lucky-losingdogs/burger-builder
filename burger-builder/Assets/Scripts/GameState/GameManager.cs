using UnityEngine;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;

    [SerializeField] private List<LevelData> m_allLevels = new List<LevelData>();
    private int m_currentLevel = 0;

    private TicketManager m_ticketManager;
    private BoardRenderer m_boardRenderer;
    private ComboManager m_comboManager;

    public static event Action<LevelData> OnLevelStart;
    public static event Action OnLevelEnd;
    public static event Action OnLevelEndEarly;

    #region Set Up
    
    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        
        m_ticketManager = FindFirstObjectByType<TicketManager>();
        m_boardRenderer = FindFirstObjectByType<BoardRenderer>();
        m_comboManager = FindFirstObjectByType<ComboManager>();
    }

    private void Start()
    {
        OnLevelStart?.Invoke(m_allLevels[m_currentLevel]);
    }

    private void OnEnable()
    {
        Timer.OnTimerFinished += HandleTimerFinished;
    }

    private void OnDisable()
    {
        Timer.OnTimerFinished -= HandleTimerFinished;
    }

    #endregion

    //if the ticket is finished set a new current ticket and clear all the tiles from the tilemap
    public void TicketCleared()
    {
        m_ticketManager.NewTicket();
        m_boardRenderer.ClearAllTiles();
        m_comboManager.TicketCleared();
    }

    //triggered when next lvl button is pressed on win menu
    public void HandleNextLevel()
    {
        //increment the current lvl index
        m_currentLevel = Mathf.Clamp(m_currentLevel++, 0, m_allLevels.Count - 1);
        StartLevel();
    }

    //triggered when retry lvl button is pressed on fail menu
    public void HandleRetryLevel()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        ClearLevel();
        OnLevelStart?.Invoke(m_allLevels[m_currentLevel]);
        GameUIManager.s_instance.ToggleBothMenus(false);
    }

    private void ClearLevel()
    {
        m_boardRenderer.ClearAllTiles();
        m_ticketManager.ResetTickets();
        OnLevelEnd?.Invoke();
    }

    //called when timer finishes
    //checks whether completed ticket num meets level requirements
    private void HandleTimerFinished()
    {
        int ticketReq = m_allLevels[m_currentLevel].GetTicketRequirement();
        if (m_ticketManager.GetCompletedTickets() >= ticketReq)
            GameUIManager.s_instance.ToggleWinMenu(true);
        else
            GameUIManager.s_instance.ToggleFailMenu(true);

        OnLevelEndEarly?.Invoke();
    }

    public TicketData GetCurrentTicket() { return m_ticketManager.GetCurrentTicket(); }

#if UNITY_EDITOR

    private void OnValidate()
    {
        m_allLevels = Utilities.LoadList<LevelData>("Levels");
    }

#endif
}