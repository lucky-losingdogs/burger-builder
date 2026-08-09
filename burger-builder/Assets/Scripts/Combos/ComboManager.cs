using System;
using UnityEngine;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    [Header("Combos")]
    [SerializeField] private int m_comboTicketRequirement = 3; //the number of tickets completed within the combo time limit 
    [SerializeField] private float m_comboTimeLimit = 5.0f; // time limit that another ticket must be completed in before losing combo
    [SerializeField] private float m_comboDecrement = 0.5f;
    
    [Header("Perks")]
    [SerializeField] private float m_perkComboRequirement = 3.0f;
    [SerializeField] private float m_perkRequirementIncrement = 0.3f;
    private float m_currentPerkRequirement = 0;

    public static event Action<float> OnPerkAchieved;

    private float m_comboTimer = 0.0f; // float representative of a timer
    private float m_currentCombo = 0.0f;
    private int m_clearedTickets = 0;
    
    private Coroutine c_comboTimer = null;
    private Coroutine c_cooldownTimer = null;
    
    private ComboUIManager m_UIManager;

    private void Awake()
    {
        m_UIManager = GetComponent<ComboUIManager>();
        if (m_UIManager == null)
            Debug.LogError("ComboUIManager not found");

        m_currentPerkRequirement = m_perkComboRequirement;
    }

    //every time a ticket is cleared, the combo timer is reset back to the time limit
    public void TicketCleared()
    {
        m_clearedTickets++;
        m_comboTimer = m_comboTimeLimit;
        
        if (c_comboTimer == null)
            c_comboTimer = StartCoroutine(C_StartComboTimer(m_comboTicketRequirement));
        
        //to start gaining combo, must clear 3 tickets within time limit
        if (m_currentCombo > 0 || m_clearedTickets >= m_comboTicketRequirement)
        {
            IncreaseCombo(m_clearedTickets);
            m_clearedTickets = 0;
        }
    }

    private void IncreaseCombo(int clearedTickets)
    {
        //if the combo increases while its cooling down, the cooldown is stopped 
        if (c_cooldownTimer != null)
        {
            StopCoroutine(c_cooldownTimer);
            c_cooldownTimer = null;
        }
        
        SetCombo(m_currentCombo + clearedTickets);
        CheckPerkRequirement(m_currentCombo);
    }
    
    //set combo value + update ui
    private void SetCombo(float value)
    {
        m_currentCombo = value;
        m_UIManager.UpdateCombo(m_currentCombo);
    }

    private void CheckPerkRequirement(float comboValue)
    {
        if (comboValue >= m_currentPerkRequirement)
        {
            m_currentPerkRequirement += m_perkComboRequirement + m_perkRequirementIncrement;
            Debug.Log($"perk combo requirement: {m_currentPerkRequirement}");
            OnPerkAchieved?.Invoke(comboValue);
        }
    }
    
    //coroutine to continuously decrease from the timer float
    private IEnumerator C_StartComboTimer(int ticketReq = 1)
    {
        while (m_comboTimer > 0)
        {
            m_comboTimer -= Time.deltaTime;
            
            yield return null;
        }
        
        //if the timer reaches 0, exits the loop and starts the combo cooldown timer
        c_comboTimer = null;
        c_cooldownTimer = StartCoroutine(C_ComboCooldownTimer());
    }

    //combo begins to slowly decrease
    private IEnumerator C_ComboCooldownTimer()
    {
        while (m_currentCombo > 0)
        {
            SetCombo(m_currentCombo - m_comboDecrement);
            m_currentPerkRequirement -= m_comboDecrement;
            yield return new WaitForSeconds(0.3f);
        }

        m_currentCombo = 0;
        m_currentPerkRequirement = m_perkComboRequirement; 
        c_cooldownTimer = null;
    }
}
