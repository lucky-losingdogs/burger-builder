using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MenuParent
{
    [SerializeField] private GameObject m_title;
    [SerializeField] private GameObject m_promptText;
    
    [Header("Values")]
    [SerializeField] private float m_titleFadeInTime = 1f;
    [SerializeField] private float m_promptFadeInDelay = 1.5f;
    [SerializeField] private float m_fadeDuration = 0.8f;

    public static event Action OnTitleLoaded;

    private bool m_isFinishedFading = false;
    private TextMeshProUGUI m_titleUI;
    private TextMeshProUGUI m_promptUI;

    #region Set Up

    private void Start()
    {
        m_titleUI = m_title.GetComponent<TextMeshProUGUI>();
        m_promptUI = m_promptText.GetComponent<TextMeshProUGUI>();
        StartDisplay();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(C_GameStartTimer());
    }

    #region Start Up Fading In
    
    //fade in title, then fade in prompt
    private IEnumerator C_GameStartTimer()
    {
        yield return new WaitForSeconds(m_titleFadeInTime);
        
        StartCoroutine(C_Fade(m_titleUI, 1, m_fadeDuration));
        
        yield return new WaitForSeconds(m_promptFadeInDelay);
        
        StartCoroutine(C_Fade(m_promptUI, 1, m_fadeDuration));

        StartCoroutine(C_EnableClicking());
    }
    
    private IEnumerator C_Fade<T>(T uiElement, float targetOpacity, float fadeDuration) where T : Graphic
    {
        m_isFinishedFading = false;
        
        float startAlpha = uiElement.color.a;
        Color startColour = uiElement.color;
        float elapsedTime = Time.deltaTime;

        while (elapsedTime < fadeDuration)
        {
            //lerp alpha to slowly change alpha over time
            elapsedTime += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetOpacity, elapsedTime / fadeDuration);
            startColour.a = alpha;
            uiElement.color = startColour;
            yield return null;
        }

        //make sure alpha is fully set to the desired opacity
        uiElement.color = ChangeOpacity(startColour, targetOpacity);

        m_isFinishedFading = true;
    }

    //keep checking if text fade in is complete before allowing input to progress
    private IEnumerator C_EnableClicking()
    {
        yield return new WaitUntil(() => m_isFinishedFading);

        ExitMenu.EnableClicking();
    }
    
    #endregion
    
    #region Toggles
    
    public override void ToggleMenuPanel()
    {
        m_mainPanel.SetActive(!m_mainPanel.activeSelf);
        ToggleTitle(m_mainPanel.activeSelf);
    }
    
    public void ToggleTitle(bool show)
    {
        m_title.SetActive(show);
    }
    
    public void TogglePrompt(bool show)
    {
        m_promptText.SetActive(show);
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
    
    //display when game loads
    //all menus are disabled and opacity of text is set to 0 to fade into 1
    protected override void StartDisplay()
    {
        base.StartDisplay();
        
        m_titleUI.color = ChangeOpacity(m_titleUI.color, 0);
        m_promptUI.color = ChangeOpacity(m_promptUI.color, 0);
    }

    private Color ChangeOpacity(Color startColour, float targetOpacity)
    {
        startColour.a = targetOpacity;
        return startColour;
    }
    
    #endregion
}
