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
    private int m_selectedLevel = (int)NullIndex.NullLevel;

    private TicketManager m_ticketManager;
    private BoardRenderer m_boardRenderer;
    private ComboManager m_comboManager;
    private PerkManager m_perkManager;
    private SpawnManager m_spawnManager;

    public static event Action<LevelData> OnLevelStart;
    public static event Action OnLevelEnd;
    public static event Action OnLevelEndEarly;
    public static event Action<GameContext> OnContextCreated;

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
        m_spawnManager = FindFirstObjectByType<SpawnManager>();
    }

    private void Start()
    {
        CreateGameContext();

        //if loaded from level select, the selected level will be an actual index instead of null index
        //set the current level to the selected level to load into the selected level
        if (m_selectedLevel != (int)NullIndex.NullLevel)
        {
            Debug.Log("Selected Level: " + m_selectedLevel);
            m_currentLevel = m_selectedLevel;
        }
        
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

    //create game context object containing managers
    //for perk logic to access for their effects
    private void CreateGameContext()
    {
        GameContext context = new GameContext(m_ticketManager, m_comboManager, m_spawnManager);
        OnContextCreated?.Invoke(context);
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
        m_ticketManager.Reset();
        m_perkManager.Reset();
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

    public TicketData GetCurrentTicket() => m_ticketManager.GetCurrentTicket();
    public void SetSelectedLevel(int index) => m_selectedLevel = index;

#if UNITY_EDITOR

    private void OnValidate()
    {
        m_allLevels = Utilities.LoadList<LevelData>("Levels");
    }

#endif
}