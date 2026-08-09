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
    
    // to signal a fade out and allow for destroy after fade, set first index to true
    // after each component of the ui finishes fading out,
    // set the equivalent bool in the array to true to allow for destroying the game object after fully fading out 
    private bool[] m_isFadeCompleted = new bool[4] {false, false, false, false}; 
    
    public void SetName(string name)
    {
        if (NameCheck(name))
            m_perkName.text = name;
        else
            m_perkName.text = m_defaultText;
    }

    public void Destroy()
    {
        if (m_isFadeCompleted[0] == false)
        {
            Debug.LogError("Cannot destroy Perk UI, not fading out");
            return;
        }
        
        StartCoroutine(C_Destroy());
    }

    public void Fade(float targetOpacity, float fadeDuration)
    {
        Image blackImg = m_blackBar.GetComponent<Image>();
        Image redImg = m_redBar.GetComponent<Image>();
        
        CheckFadeOut(targetOpacity);
        
        StartCoroutine(C_Fade<Image>(blackImg, targetOpacity, fadeDuration, 1));
        StartCoroutine(C_Fade<Image>(redImg, targetOpacity, fadeDuration, 2));
        StartCoroutine(C_Fade<TextMeshProUGUI>(m_perkName, targetOpacity, fadeDuration, 3));
    }

    private IEnumerator C_Fade<T>(T uiElement, float targetOpacity, float fadeDuration, int index) where T : Graphic
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
        
        if (m_isFadeCompleted[0] == true)
            m_isFadeCompleted[index] = true;
    }

    private IEnumerator C_Destroy()
    {
        bool allFadesComplete = false;

        while (!allFadesComplete)
        {
            for (int i = 1;  i < m_isFadeCompleted.Length; i++)
            {
                if (m_isFadeCompleted[i] == false)
                {
                    allFadesComplete = false;
                    break;
                }
                else
                {
                    allFadesComplete = true;
                }
            }

            yield return null;
        }
        
        Destroy(gameObject);
    }

    //setting first element in array to true to allow for
    //destroying the game object after fade is completed
    private void CheckFadeOut(float targetOpacity)
    {
        if (targetOpacity <= 0)
            m_isFadeCompleted[0] = true;
    }
    
    public GameObject GetBlackBar() { return m_blackBar; }
    public GameObject GetRedBar() { return m_redBar; }
    private bool NameCheck(string name) { return name.Length > 0; }
}
