using UnityEngine;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelManager : MonoBehaviour
{
    public static LevelManager s_instance;

    [SerializeField] private List<LevelData> m_allLevels = new List<LevelData>();
    private int m_currentLevel = 0;
    private int m_selectedLevel = (int)NullIndex.NullLevel;

    private TicketManager m_ticketManager;
    private BoardRenderer m_boardRenderer;
    private ComboManager m_comboManager;
    private PerkManager m_perkManager;
    private SpawnManager m_spawnManager;
    private TicketRequirementUI m_ticketRequirementUI;

    public static event Action<LevelData> OnLevelStart;
    public static event Action OnLevelEnd;
    public static event Action OnLevelEndEarly;
    public static event Action<GameContext> OnContextCreated;
    public static event Action OnTicketCleared;

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
        m_ticketRequirementUI = FindFirstObjectByType<TicketRequirementUI>();
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

        HandleLevelStart();
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
    public void HandleTicketCleared()
    {
        OnTicketCleared?.Invoke();
    }

    //triggered when next lvl button is pressed on win menu
    //increases current level index so the next level is loaded
    public void HandleNextLevel()
    {
        //increment the current lvl index
        m_currentLevel = Mathf.Clamp(m_currentLevel + 1, 0, m_allLevels.Count);
        StartLevel();
    }

    //triggered when retry lvl button is pressed on fail menu
    //does not increase current level index so the same level is loaded again
    public void HandleRetryLevel()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        ClearLevel();
        HandleLevelStart();
        LevelUIManager.s_instance.ToggleBothMenus(false);
    }

    private void ClearLevel()
    {
        OnLevelEnd?.Invoke();
    }

    //called when timer finishes
    //checks whether completed ticket num meets level requirements
    private void HandleTimerFinished()
    {
        int ticketReq = m_allLevels[m_currentLevel].GetTicketRequirement();
        if (m_ticketManager.GetCompletedTickets() >= ticketReq)
            LevelUIManager.s_instance.ToggleWinMenu(true);
        else
            LevelUIManager.s_instance.ToggleFailMenu(true);

        OnLevelEndEarly?.Invoke();
    }

    private void HandleLevelStart()
    {
        Debug.Log(m_currentLevel);
        if (m_currentLevel > m_allLevels.Count)
            m_currentLevel = m_allLevels.Count;
                
        OnLevelStart?.Invoke(m_allLevels[m_currentLevel]);
    }

    public TicketData GetCurrentTicket() => m_ticketManager.GetCurrentTicket();
    public void SetSelectedLevel(int index) => m_selectedLevel = index;
    public int GetCurrentLevel() => m_currentLevel;

#if UNITY_EDITOR

    private void OnValidate()
    {
        m_allLevels = Utilities.LoadList<LevelData>("Levels");
    }

#endif
}