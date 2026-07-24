using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ComboUIManager : MonoBehaviour
{
    [SerializeField] private Image m_barImage;
    
    [Header("Bar Set Up")]
    [SerializeField] private List<float> m_barFillSegments = new List<float>();
    [SerializeField] private List<bool> m_updateFillAmounts = new List<bool>();
    [SerializeField] private bool m_resetSegments = false;

    private void Awake()
    {
        if (m_barImage == null)
            Debug.LogError("No Bar Image!");
    }

    public void UpdateCombo(float combo)
    {
        //m_comboText.text = $"{combo}";
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        UpdateButtons();
        UpdateFillSegments();
        ResetSegments();
    }

    private void UpdateButtons()
    {
        if (m_barFillSegments.Count != m_updateFillAmounts.Count)
            m_updateFillAmounts = new List<bool>(new bool [m_barFillSegments.Count]);
    }

    private void UpdateFillSegments()
    {
        for (int i = 0; i < m_updateFillAmounts.Count; i++)
        {
            if (m_updateFillAmounts[i] == true)
            {
                if (m_barImage != null && m_barImage.type == Image.Type.Filled)
                    m_barFillSegments[i] = m_barImage.fillAmount;

                m_updateFillAmounts[i] = false;
            }
        }
    }

    private void ResetSegments()
    {
        if (m_resetSegments)
        {
            for (int i = 0; i < m_barFillSegments.Count; i++)
            {
                m_barFillSegments[i] = 0;
            }
            m_resetSegments = false;
        }
    }

#endif
}
