using UnityEngine;

public class LevelUIManager : MonoBehaviour
{
    public static LevelUIManager s_instance;

    [SerializeField] private GameObject m_winMenu;
    [SerializeField] private GameObject m_failMenu;

    #region Set Up

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;

        if (m_winMenu == null)
            Debug.LogError("Win screen UI is null!");
        if (m_failMenu == null)
            Debug.LogError("Fail screen UI is null!");
    }

    private void Start()
    {
        ToggleBothMenus(false);
    }

    #endregion

    public void ToggleWinMenu(bool show)
    {
        ToggleMenu(show, m_winMenu);
    }

    public void ToggleFailMenu(bool show)
    {
        ToggleMenu(show, m_failMenu);
    }

    public void ToggleBothMenus(bool show)
    {
        ToggleMenu(show, m_failMenu);
        ToggleMenu(show, m_winMenu);
    }

    private void ToggleMenu(bool show, GameObject menu)
    {
        if (show)
        {
            menu.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {
            menu.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
}
