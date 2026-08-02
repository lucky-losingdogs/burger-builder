using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_perkName;
    [SerializeField] private GameObject m_blackBar;
    [SerializeField] private GameObject m_redBar;

    private string m_defaultText = "Default Name";
    
    public void SetName(string name)
    {
        if (NameCheck(name))
            m_perkName.text = name;
        else
            m_perkName.text = m_defaultText;
    }

    public void Fade(float targetOpacity, float fadeDuration)
    {
        Image blackImg = m_blackBar.GetComponent<Image>();
        Image redImg = m_redBar.GetComponent<Image>();
        
        StartCoroutine(C_Fade<Image>(blackImg, targetOpacity, fadeDuration));
        StartCoroutine(C_Fade<Image>(redImg, targetOpacity, fadeDuration));
        StartCoroutine(C_Fade<TextMeshProUGUI>(m_perkName, targetOpacity, fadeDuration));
    }

    private IEnumerator C_Fade<T>(T uiElement, float targetOpacity, float fadeDuration) where T : Graphic
    {
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
        startColour.a = targetOpacity;
        uiElement.color = startColour;
    }
    
    public GameObject GetBlackBar() { return m_blackBar; }
    public GameObject GetRedBar() { return m_redBar; }
    private bool NameCheck(string name) { return name.Length > 0; }
}
