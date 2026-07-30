using System;
using UnityEngine;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TilemapUI m_tilemapUI;
    private TicketData m_data;

    public void Populate(TicketData data)
    {
        m_data = data;
        
        m_icon.sprite = data.GetSprite();

        m_tilemapUI.SetTilemap();
        m_tilemapUI.SetGridDiagram(data.GetItems());
        ToggleIcon(true);
    }

    public void ToggleIcon(bool showIcon)
    {
        if (showIcon)
        {
            m_icon.gameObject.SetActive(true);
            m_tilemapUI.Hide();
        }
        else
        {
            m_tilemapUI.Show();
            m_icon.gameObject.SetActive(false);
        }
    }

    public TicketData GetData() { return m_data; }
}
