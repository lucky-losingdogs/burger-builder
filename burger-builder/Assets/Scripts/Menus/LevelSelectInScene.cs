using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectInScene : MenuParent
{
    [Header("Level Select")]
    [SerializeField] private GameObject m_levelButtonPrefab;
    [SerializeField] private Transform m_levelButtonParent;
    
    private Button[] m_levelButtons;
    [SerializeField] private List<LevelData> m_allLevels = new List<LevelData>();

    #region  Set up
    
    protected void Awake()
    {
        m_levelButtons = SpawnLevelButtons().ToArray();
    }

    private List<Button> SpawnLevelButtons()
    {
        List<Button> tempButtonList = new List<Button>();

        //for each level spawn a button
        for (int i = 0; i < m_allLevels.Count; i++)
        {
            GameObject buttonObject = Instantiate(m_levelButtonPrefab, m_levelButtonParent);
            if (buttonObject == null)
                continue;
            
            Button button = buttonObject.GetComponent<Button>();
            if (button == null)
                continue;
            
            LevelButtonUI ui = buttonObject.GetComponent<LevelButtonUI>();
            if (ui == null)
                continue;
            
            //add to button list
            tempButtonList.Add(button);
            
            //change the button's text to read the correct level number
            ui.SetLevelNumber(i + 1);
        }

        return tempButtonList;
    }

    //each button on click triggers the function and passes the index it is in the array as the level index
    protected void OnEnable()
    {
        for (int i = 0; i < m_levelButtons.Length; i++)
        {
            int levelIndex = i;
            m_levelButtons[i].onClick.AddListener(() => OnButtonClick(levelIndex));
        }
    }

    protected void OnDisable()
    {
        foreach (Button button in m_levelButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    #endregion
    
    //change the level while in the main scene
    //no inter-scene persistence required
    protected virtual void OnButtonClick(int index)
    {
        Debug.Log("Switch to level: " + index);
        GameManager.s_instance.SetSelectedLevel(index);
    }
    
#if UNITY_EDITOR

    protected void OnValidate()
    {
        m_allLevels = Utilities.LoadList<LevelData>("Levels");
    }

#endif
}
