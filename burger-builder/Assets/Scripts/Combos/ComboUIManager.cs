using System;
using UnityEngine;
using UnityEngine.UI;

public class ComboUIManager : MonoBehaviour
{
    [SerializeField] private Image m_barImage;

    private void Awake()
    {
        if (m_barImage == null)
            Debug.LogError("No Bar Image!");
        else
        {
            m_barImage.fillAmount = 0.05f;
        }
    }

    public void UpdateCombo(float combo)
    {
        Debug.Log(combo);
        m_barImage.fillAmount = combo / 10;
    }
}
