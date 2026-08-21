using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MenuParent
{
    [SerializeField] private TitleAnimation m_titleAnimation;
    
    public static event Action OnTitleLoaded;
    public static event Action<int> OnContinueButtonPressed;

    #region Set Up

    private void Start()
    {
        if (TitleAnimationPlayed.s_instance.GetTitlePlayed())
        {
            DefaultDisplay();
        }
        else
        {
            StartDisplay();
            StartCoroutine(m_titleAnimation.C_GameStartTimer());
        }
    }

    #endregion

    public void HandleContinueButton()
    {
        int highestLevel = SaveManager.s_instance.GetSaveData().highestLevel;
        OnContinueButtonPressed?.Invoke(highestLevel);
    }
    
    #region Toggles
    
    public override void ToggleMenuPanel()
    {
        m_mainPanel.SetActive(!m_mainPanel.activeSelf);
        ToggleTitle(m_mainPanel.activeSelf);
    }
    
    public void ToggleTitle(bool show)
    {
        m_titleAnimation.ToggleTitle(show);
    }
    
    public void TogglePrompt(bool show)
    {
        m_titleAnimation.TogglePrompt(show);
    }

    //the default display with the menu panel and title visible
    //used when clicking back to display all buttons
    protected override void DefaultDisplay()
    {
        ToggleMenuPanel(true);
        ToggleTitle(true);
        ToggleOptionsPanel(false);
        ToggleLevelSelectPanel(false);
        TogglePrompt(false);
    }
    
    protected override void StartDisplay()
    {
        base.StartDisplay();
        m_titleAnimation.HideTitle();
    }
    
    #endregion
}
