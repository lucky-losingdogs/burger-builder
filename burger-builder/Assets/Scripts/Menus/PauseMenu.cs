using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MenuParent
{
    [SerializeField] private GameObject m_menuBackground;
    
    [Header("Input")]
    [SerializeField] private InputActionReference m_openMenu;

    #region Set Up

    private void Start()
    {
        StartDisplay();
    }
    
    private void OnEnable()
    {
        base.OnEnable();
        
        m_openMenu.action.Enable();
        m_openMenu.action.performed += HandleOpenMenu;
    }

    private void OnDisable()
    {
        base.OnDisable();
        
        m_openMenu.action.Disable();
        m_openMenu.action.performed -= HandleOpenMenu;
    }
    
    #endregion

    private void HandleOpenMenu(InputAction.CallbackContext context)
    {
        ToggleMenuBackground();
        DefaultDisplay();
    }
    
    protected override void DefaultDisplay()
    {
        ToggleMenuPanel(true);
        ToggleOptionsPanel(false);
        ToggleLevelSelectPanel(false);
    }

    protected override void StartDisplay()
    {
        ToggleMenuPanel(true);
        ToggleOptionsPanel(false);
        ToggleLevelSelectPanel(false);
        ToggleMenuBackground(false);
    }

    public void ToggleMenuBackground()
    {
        m_menuBackground.SetActive(!m_menuBackground.activeSelf);
        SetTimeScale(m_menuBackground.activeSelf);
    }
    
    private void ToggleMenuBackground(bool show)
    {
        m_menuBackground.SetActive(show);
    }

    private void SetTimeScale(bool menuActive)
    {
        if (menuActive)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
