using System;
using UnityEngine;
using UnityEngine.UI;

public class ComboUIManager : MonoBehaviour
{
    [SerializeField] private Image m_barImage;
    
    private float m_perkRequirement = 0;
    
    private float m_comboAtPerk = 0.0f;
    private float m_currentCombo = 0.0f;

    private void Awake()
    {
        if (m_barImage == null)
            Debug.LogError("No Bar Image!");
        else
        {
            m_barImage.fillAmount = 0.05f;
        }
    }

    //update bar image's fill amount to match current combo
    public void UpdateCombo(float combo)
    {
        if (m_perkRequirement <= 0f)
        {
            m_barImage.fillAmount = 0.05f;
            return;
        }

        m_currentCombo = combo;
        
        float perkProgress = combo - m_comboAtPerk;
        float perkRequirement = m_perkRequirement - m_comboAtPerk;
        float fill = perkProgress / perkRequirement;
        
        m_barImage.fillAmount = Mathf.Clamp(fill, 0.05f, 1f);
    }

    public void SetPerkRequirement(float requirement)
    {
        m_comboAtPerk = m_currentCombo;
        m_perkRequirement = requirement;
        m_barImage.fillAmount = 0.05f;
    }
}
