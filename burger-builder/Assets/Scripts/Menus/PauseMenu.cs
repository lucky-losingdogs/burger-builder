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
        MenuClicking.EnableClicking();
        StartDisplay();
    }
    
    private void OnEnable()
    {
        base.OnEnable();
        
        m_openMenu.action.Enable();
        m_openMenu.action.performed += HandleOpenMenu;

        LevelManager.OnLevelEnd += StartDisplay;
    }

    private void OnDisable()
    {
        base.OnDisable();
        
        m_openMenu.action.Disable();
        m_openMenu.action.performed -= HandleOpenMenu;
        
        LevelManager.OnLevelEnd -= StartDisplay;
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
        ToggleMenuPanel(false);
        ToggleOptionsPanel(false);
        ToggleLevelSelectPanel(false);
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
            GameTime.RequestPause();
        else
            GameTime.ReleasePause();
        Debug.Log(Time.timeScale);
    }
}
