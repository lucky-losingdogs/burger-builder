using System;
using TMPro;
using UnityEngine;

public class TicketRequirementUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ticketRequirementUI;
    
    private int m_ticketRequirement;
    private int m_currentTicketRequirement;

    #region Set Up

    private void OnEnable()
    {
        LevelManager.OnLevelStart += HandleLevelStart;
        LevelManager.OnTicketCleared += UpdateRequirementUI;
        LevelManager.OnTicketsCleared += UpdateRequirementUI;
    }

    private void OnDisable()
    {
        LevelManager.OnLevelStart -= HandleLevelStart;
        LevelManager.OnTicketCleared -= UpdateRequirementUI;
        LevelManager.OnTicketsCleared -= UpdateRequirementUI;
    }

    private void HandleLevelStart(LevelData levelData)
    {
        m_ticketRequirement = levelData.GetTicketRequirement();
        m_currentTicketRequirement = m_ticketRequirement;
        
        m_ticketRequirementUI.text = m_ticketRequirement.ToString();
    }

    #endregion

    private void UpdateRequirementUI()
    {
        m_currentTicketRequirement = Mathf.Clamp(m_currentTicketRequirement - 1, 0, m_ticketRequirement);
        m_ticketRequirementUI.text = m_currentTicketRequirement.ToString();
    }
    
    private void UpdateRequirementUI(int clearedTickets)
    {
        m_currentTicketRequirement = Mathf.Clamp(m_currentTicketRequirement - clearedTickets, 0, m_ticketRequirement);
        m_ticketRequirementUI.text = m_currentTicketRequirement.ToString();
    }
}
