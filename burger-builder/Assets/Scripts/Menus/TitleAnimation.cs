using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject m_title;
    [SerializeField] private GameObject m_promptText;
    [SerializeField] private GameObject m_animationPlayedPrefab;
    
    [Header("Values")]
    [SerializeField] private float m_titleFadeInTime = 1f;
    [SerializeField] private float m_promptFadeInDelay = 1.5f;
    [SerializeField] private float m_fadeDuration = 0.8f;

    private bool m_isFinishedFading = false;
    private TextMeshProUGUI m_titleUI;
    private TextMeshProUGUI m_promptUI;

    private void Awake()
    {
        m_titleUI = m_title.GetComponent<TextMeshProUGUI>();
        m_promptUI = m_promptText.GetComponent<TextMeshProUGUI>();
    }

    #region Start Up Fading In
    
    //fade in title, then fade in prompt
    public IEnumerator C_GameStartTimer()
    {
        yield return new WaitForSeconds(m_titleFadeInTime);
        
        StartCoroutine(C_Fade(m_titleUI, 1, m_fadeDuration));
        
        yield return new WaitForSeconds(m_promptFadeInDelay);
        
        StartCoroutine(C_Fade(m_promptUI, 1, m_fadeDuration));

        StartCoroutine(C_TitleAnimationFinished());
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
    private IEnumerator C_TitleAnimationFinished()
    {
        yield return new WaitUntil(() => m_isFinishedFading);

        MenuClicking.EnableClicking();
        
        if (TitleAnimationPlayed.s_instance != null)
            TitleAnimationPlayed.s_instance.SetTitlePlayed(true);
    }
    
    #endregion

    public void HideTitle()
    {
        m_titleUI.color = ChangeOpacity(m_titleUI.color, 0);
        m_promptUI.color = ChangeOpacity(m_promptUI.color, 0);
    }
    
    private Color ChangeOpacity(Color startColour, float targetOpacity)
    {
        startColour.a = targetOpacity;
        return startColour;
    }
    
    public void ToggleTitle(bool show)
    {
        m_title.SetActive(show);
    }
    
    public void TogglePrompt(bool show)
    {
        m_promptText.SetActive(show);
    }
}
