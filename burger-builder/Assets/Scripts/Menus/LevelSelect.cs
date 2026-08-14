using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelSelect : MenuParent
{
    [SerializeField] private GameObject m_targetLevelPrefab;
    
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

    //spawn a DontDestroyOnLoad object carrying the level index
    //then load the main scene where the object passes the level index to the game manager
    protected virtual void OnButtonClick(int index)
    {
        GameObject targetLevelObject = Instantiate(m_targetLevelPrefab);
        if (targetLevelObject == null)
            return;
        
        TargetLevel targetLevel = targetLevelObject.GetComponent<TargetLevel>();
        if (targetLevel == null)
            return;
        
        targetLevel.SetTargetLevelIndex(index);
        
        LoadTargetScene();
    }
    
#if UNITY_EDITOR

    protected void OnValidate()
    {
        m_allLevels = Utilities.LoadList<LevelData>("Levels");
    }

#endif
}
