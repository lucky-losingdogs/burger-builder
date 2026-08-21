using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboManager : MonoBehaviour
{
    [Header("Combos")]
    [SerializeField] private float m_comboTimeLimit = 5.0f; // time limit that another ticket must be completed in before losing combo
    [SerializeField] private float m_comboDecrement = 0.5f;
    
    [Header("Perks")]
    [SerializeField] private float m_perkComboRequirement = 3.0f; //req at beginning
    [SerializeField] private float m_perkRequirementIncrement = 0.3f;
    private float m_currentPerkRequirement = 0; //changes through level

    public static event Action<float> OnPerkAchieved;

    private int m_clearedTickets = 0;
    private float m_comboTimer = 0.0f; // float representative of a timer
    private float m_currentCombo = 0.0f;
    
    private float m_multiplier = 1.0f;
    
    private Coroutine c_comboTimer = null;
    private Coroutine c_cooldownTimer = null;
    
    private ComboUIManager m_UIManager;
    
    #region Perk Variables

    private float m_originalComboTimeLimit;
    private float m_originalComboDecrement;
    private const float m_originalMultiplier = 1.0f;
    
    private List<float> m_comboTimeLimitModifiers = new List<float>();
    private List<float> m_comboDecrementModifiers = new List<float>();
    private List<float> m_multiplierModifiers = new List<float>();
    
    #endregion

    #region Set Up
    
    private void Awake()
    {
        m_UIManager = GetComponent<ComboUIManager>();
        if (m_UIManager == null)
            Debug.LogError("ComboUIManager not found");

        m_currentPerkRequirement = m_perkComboRequirement;

        m_originalComboTimeLimit = m_comboTimeLimit;
        m_originalComboDecrement = m_comboDecrement;
    }

    private void Start()
    {
        UpdateUIMax();
    }

    private void OnEnable()
    {
        LevelManager.OnTicketCleared += HandleTicketCleared;
    }

    private void OnDisable()
    {
        LevelManager.OnTicketCleared -= HandleTicketCleared;
    }

    #endregion

    //every time a ticket is cleared, the combo timer is reset back to the time limit
    private void HandleTicketCleared()
    {
        m_clearedTickets++;
        m_comboTimer = m_comboTimeLimit;
        
        if (c_comboTimer == null)
            c_comboTimer = StartCoroutine(C_StartComboTimer());
        
        //to start gaining combo, must clear 3 tickets within time limit
        if (m_currentCombo > 0 || m_clearedTickets >= 0)
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
        
        SetCombo(m_multiplier * (m_currentCombo + clearedTickets));
        CheckPerkRequirement(m_currentCombo);
    }
    
    //set combo value + update ui
    private void SetCombo(float value)
    {
        m_currentCombo = value;
        m_UIManager.UpdateCombo(m_currentCombo);
    }

    //if the combo meets the requirement to trigger a perk, invoke event triggering perk
    private void CheckPerkRequirement(float comboValue)
    {
        if (comboValue >= m_currentPerkRequirement)
        {
            //increase perk requirement to be greater than combo
            m_currentPerkRequirement += m_perkComboRequirement + m_perkRequirementIncrement;
            OnPerkAchieved?.Invoke(comboValue);
            UpdateUIMax();
        }
    }

    private void UpdateUIMax()
    {
        m_UIManager.SetPerkRequirement(m_currentPerkRequirement);
    }
    
    #region Perk Behaviour

    //<summary>
    //public functions are called by perk logic
    //</summary>
    
    #region Combo Decrement
    
    public void DecreaseDecrement(float duration, float value)
    {
        StartCoroutine(PerkManager.C_EffectDuration(AddComboDecrementModifier, RemoveComboDecrementModifier, duration, value));
    }
    
    private void AddComboDecrementModifier(float value)
    {
        m_comboDecrementModifiers.Add(value);
        RecalculateComboDecrement();
    }

    private void RemoveComboDecrementModifier(float value)
    {
        m_comboDecrementModifiers.Remove(value);
        RecalculateComboDecrement();
    }

    private void RecalculateComboDecrement()
    {
        m_comboDecrement = PerkManager.Recalculate(m_comboDecrementModifiers, m_originalComboDecrement);
    }
    
    #endregion

    #region Combo Time Limit
    
    public void IncreaseComboTimeLimit(float duration, float value)
    {
        //change the combo time limit and restore after the perk's duration
        StartCoroutine(PerkManager.C_EffectDuration(AddComboTimeLimitModifier, RemoveComboTimeLimitModifier, duration, value));
        
        //update the current running combo timer based on the value, but don't restore it after duration
        //as after duration it should not be reset 
        UpdateComboTimer(value);
    }

    private void UpdateComboTimer(float value)
    {
        if (m_comboTimer > 0f)
        {
            m_comboTimer = Mathf.Max(0f, m_comboTimer + value);
        }
    }
    
    private void AddComboTimeLimitModifier(float value)
    {
        m_comboTimeLimitModifiers.Add(value);
        RecalculateComboTimeLimit();
    }

    private void RemoveComboTimeLimitModifier(float value)
    {
        m_comboTimeLimitModifiers.Remove(value);
        RecalculateComboTimeLimit();
    }

    private void RecalculateComboTimeLimit()
    {
        m_comboTimeLimit = PerkManager.Recalculate(m_comboTimeLimitModifiers, m_originalComboTimeLimit);
    }
    
    #endregion
    
    #region Multiplier
    
    public void IncreaseMultiplier(float duration, float value)
    {
        StartCoroutine(PerkManager.C_EffectDuration(AddMultiplierModifier, RemoveMultiplierModifier, duration, value));
    }
    
    private void AddMultiplierModifier(float value)
    {
        m_multiplierModifiers.Add(value);
        RecalculateMultiplier();
    }

    private void RemoveMultiplierModifier(float value)
    {
        m_multiplierModifiers.Remove(value);
        RecalculateMultiplier();
    }

    private void RecalculateMultiplier()
    {
        m_multiplier = PerkManager.Recalculate(m_multiplierModifiers, m_originalMultiplier);
    }
    
    #endregion

    #endregion
    
    //coroutine to continuously decrease from the timer float
    private IEnumerator C_StartComboTimer()
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
            m_currentPerkRequirement -= m_comboDecrement;
            SetCombo(m_currentCombo - m_comboDecrement);
            yield return new WaitForSeconds(0.3f);
        }

        m_currentCombo = 0;
        m_currentPerkRequirement = m_perkComboRequirement; 
        c_cooldownTimer = null;
    }
}
