using UnityEngine;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private int m_comboTicketRequirement = 3; //the number of tickets completed within the combo time limit 
    [SerializeField] private float m_comboTimeLimit = 5.0f;
    [SerializeField] private float m_comboDecrement = 0.5f;

    private float m_comboTimer = 0.0f;
    private float m_currentCombo = 0.0f;
    private int m_clearedTickets = 0;
    
    private Coroutine c_timer = null;
    private Coroutine c_cooldownTimer = null;
    
    private ComboUIManager m_UIManager;

    private void Awake()
    {
        m_UIManager = GetComponent<ComboUIManager>();
        if (m_UIManager == null)
            Debug.LogError("ComboUIManager not found");
    }

    public void TicketCleared()
    {
        m_clearedTickets++;
        m_comboTimer = m_comboTimeLimit;
        
        if (c_timer == null)
            c_timer = StartCoroutine(C_StartComboTimer(m_comboTicketRequirement));
        
        if (m_currentCombo > 0 || m_clearedTickets >= m_comboTicketRequirement)
        {
            IncreaseCombo(m_clearedTickets);
            m_clearedTickets = 0;
        }
    }

    private void IncreaseCombo(int clearedTickets)
    {
        if (c_cooldownTimer != null)
        {
            StopCoroutine(c_cooldownTimer);
            c_cooldownTimer = null;
        }
        
        SetCombo(m_currentCombo + clearedTickets);
    }
    
    private void SetCombo(float value)
    {
        m_currentCombo = value;
        m_UIManager.UpdateCombo(m_currentCombo);
    }
    
    private IEnumerator C_StartComboTimer(int ticketReq = 1)
    {
        while (m_comboTimer > 0)
        {
            m_comboTimer -= Time.deltaTime;
            
            yield return null;
        }
        
        c_timer = null;
        c_cooldownTimer = StartCoroutine(C_ComboCooldownTimer());
    }

    private IEnumerator C_ComboCooldownTimer()
    {
        while (m_currentCombo > 0)
        {
            SetCombo(m_currentCombo - m_comboDecrement);
            yield return new WaitForSeconds(0.3f);
        }

        m_currentCombo = 0;
        c_cooldownTimer = null;
    }
}
