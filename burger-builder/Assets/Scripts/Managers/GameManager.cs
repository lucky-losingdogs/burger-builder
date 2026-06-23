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

    public event Action<LevelData> OnLevelStart;

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
        m_ticketManager.Enable();

        OnLevelStart?.Invoke(m_allLevels[m_currentLevel]);
    }

    //if the ticket is finished set a new current ticket and clear all the tiles from the tilemap
    public void TicketCleared()
    {
        m_ticketManager.NewTicket();
        m_boardRenderer.ClearAllTiles();
    }

    public void AllTicketsCleared()
    {
        m_currentLevel++;
    }

    public void OutOfTime()
    {

    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        m_allLevels = Utilities.LoadList<LevelData>("Levels");
    }

#endif
}