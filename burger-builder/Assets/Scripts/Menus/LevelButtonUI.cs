using TMPro;
using UnityEngine;

public class LevelButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_levelNumber;

    private string defaultText = "Level ";
    
    public void SetLevelNumber(int levelNumber)
    {
        m_levelNumber.text = defaultText + levelNumber.ToString();
    }
}
