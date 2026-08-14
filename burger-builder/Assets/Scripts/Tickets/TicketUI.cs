using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TilemapUI m_tilemapUI;
    
    [SerializeField] private TextMeshProUGUI m_ticketNumber;
    [SerializeField] private RectTransform m_stripe;
    
    private TicketData m_data;
    private Vector2 m_originalSize;

    private void Awake()
    {
        RectTransform parentRect = GetComponentInParent<RectTransform>();
        m_originalSize = parentRect.sizeDelta;
    }

    public void SetData(TicketData data)
    {
        m_data = data;
        m_icon.sprite = data.GetSprite();

        m_tilemapUI.SetTilemap();
        m_tilemapUI.SetGridDiagram(data.GetItems());
        ToggleIcon(true);
    }

    public void SetNumber(string number)
    {
        m_ticketNumber.text = number;
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

    public void RebuildLayout(Vector2 size, Vector2 position)
    {
        m_tilemapUI.RebuildLayout(size, position);
        float yScale = size.y / m_originalSize.y;
        m_stripe.sizeDelta = new Vector2(m_stripe.sizeDelta.x, m_stripe.rect.height * yScale);
    }
}
 