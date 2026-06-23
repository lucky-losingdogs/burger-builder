using UnityEngine;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    private Transform m_parent;
    private TicketData m_data;

    public void Populate(TicketData data, Transform parent)
    {
        m_data = data;
        m_icon.sprite = data.GetSprite();
        m_parent = parent;
    }

    public void SetParent()
    {
        transform.SetParent(m_parent, false);
    }

    public TicketData GetData() { return m_data; }
}
