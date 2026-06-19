using UnityEngine;
using UnityEngine.UI;

public class TicketUI : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    private Transform m_parent;

    public void Populate(Sprite sprite, Transform parent)
    {
        m_icon.sprite = sprite;
        m_parent = parent;
    }

    public void SetParent()
    {
        transform.SetParent(m_parent, false);
    }
}
