using UnityEngine;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    private TicketData m_data;

    public void Populate(TicketData data)
    {
        m_data = data;
        m_icon.sprite = data.GetSprite();
    }

    public void SetSprite(Sprite sprite)
    {
        m_icon.sprite = sprite;
    }

    public TicketData GetData() { return m_data; }
}
