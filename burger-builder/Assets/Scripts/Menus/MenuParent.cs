using System;
using UnityEngine;

public class MenuParent : MonoBehaviour
{
    [SerializeField] protected SceneData m_targetScene;
    private int m_targetSceneIndex;
    
    [Header("Object References")]
    [SerializeField] protected GameObject m_mainPanel;
    [SerializeField] protected GameObject m_optionsPanel;
    [SerializeField] protected GameObject m_levelSelectPanel;

    protected virtual void OnEnable()
    {
        ExitMenu.OnClickOff += HandleClickOff;
    }

    protected virtual void OnDisable()
    {
        ExitMenu.OnClickOff -= HandleClickOff;
    }

    protected void Awake()
    {
        //get the scene index of the new scene that can be loaded
        m_targetSceneIndex = m_targetScene.GetIndex();
    }

    //quits application
    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    //loads a new level
    public void LoadTargetScene()
    {
        if (m_targetSceneIndex >= 0)
        {
            ResetTimeScale();
            LoadScene.s_instance.StartLoading(m_targetSceneIndex);
            m_targetSceneIndex = (int)NullIndex.NullLevel;
        }
    }
    
    private void ResetTimeScale()
    {
        Time.timeScale = 1;
        Debug.Log(Time.timeScale);
    }
    
    //when player clicks on background or back button,
    //set menus to the default menu display
    public virtual void HandleClickOff()
    {
        DefaultDisplay();
    }

    #region Toggles

    public virtual void ToggleMenuPanel()
    {
        m_mainPanel.SetActive(!m_mainPanel.activeSelf);
    }
    
    public void ToggleMenuPanel(bool show)
    {
        m_mainPanel.SetActive(show);
    }

    public void ToggleOptionsPanel()
    {
        m_optionsPanel.SetActive(!m_optionsPanel.activeSelf);
    }
    
    public void ToggleOptionsPanel(bool show)
    {
        m_optionsPanel.SetActive(show);
    }
    
    public void ToggleLevelSelectPanel()
    {
        m_levelSelectPanel.SetActive(!m_levelSelectPanel.activeSelf);
    }
    
    public void ToggleLevelSelectPanel(bool show)
    {
        m_levelSelectPanel.SetActive(show);
    }

    protected virtual void DefaultDisplay()
    {
        
    }

    protected virtual void StartDisplay()
    {
        ToggleMenuPanel(false);
        ToggleOptionsPanel(false);
        ToggleLevelSelectPanel(false);
    }

    #endregion
}
