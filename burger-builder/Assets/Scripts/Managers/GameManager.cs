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

    public static event Action<LevelData> OnLevelStart;
    public static event Action OnLevelEnd;

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
    }

    private void Start()
    {
        OnLevelStart?.Invoke(m_allLevels[m_currentLevel]);
    }

    //if the ticket is finished set a new current ticket and clear all the tiles from the tilemap
    public void TicketCleared()
    {
        m_ticketManager.NewTicket();
        m_boardRenderer.ClearAllTiles();
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

    public TicketData GetCurrentTicket() { return m_ticketManager.GetCurrentTicket(); }

#if UNITY_EDITOR

    private void OnValidate()
    {
        m_allLevels = Utilities.LoadList<LevelData>("Levels");
    }

#endif
}