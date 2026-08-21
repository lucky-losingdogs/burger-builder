using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectInScene : MenuParent, ILoadableSaveData
{
    [Header("Level Select")]
    [SerializeField] private GameObject m_levelButtonPrefab;
    [SerializeField] private Transform m_levelButtonParent;
    
    private Button[] m_levelButtons = Array.Empty<Button>();
    [SerializeField] private List<LevelData> m_allLevels = new List<LevelData>();
    [SerializeField] private List<LevelData> m_unlockedLevels = new List<LevelData>();

    #region  Set up

    private List<Button> SpawnLevelButtons()
    {
        List<Button> tempButtonList = new List<Button>();
        ClearLevelButtons();

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

    private void HideLevelButtons()
    {
        for (int i = m_unlockedLevels.Count; i < m_levelButtons.Length; i++)
        {
            m_levelButtons[i].gameObject.SetActive(false);
        }
    }

    //each button on click triggers the function and passes the index it is in the array as the level index
    protected override void OnEnable()
    {
        base.OnEnable();
        SaveManager.OnSaveLoaded += HandleSaveLoaded;
        LevelManager.OnLevelEnd += Reset;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SaveManager.OnSaveLoaded -= HandleSaveLoaded;
        LevelManager.OnLevelEnd -= Reset;
    }

    public void HandleSaveLoaded(SaveData saveData)
    {
        int highestLevel = saveData.highestLevel;
        // int highestLevel = 1;
        m_allLevels = (this as ILoadableSaveData).GetAllLevels();
        m_unlockedLevels = (this as ILoadableSaveData).GetUnlockedLevels(highestLevel);
        
        m_levelButtons = SpawnLevelButtons().ToArray();
        BindLevelButtons();
        HideLevelButtons();
    }
    
    private void BindLevelButtons()
    {
        for (int i = 0; i < m_levelButtons.Length; i++)
        {
            int levelIndex = i;
            m_levelButtons[i].onClick.AddListener(() => OnButtonClick(levelIndex));
        }
    }

    private void OnDestroy()
    {
        ClearLevelButtons();
    }

    #endregion
    
    //change the level while in the main scene
    //no inter-scene persistence required
    protected virtual void OnButtonClick(int index)
    {
        Debug.Log("Switch to level: " + index);
        LevelManager.s_instance.SetSelectedLevel(index);
        LevelManager.s_instance.ReloadLevel();
    }

    private void ClearLevelButtons()
    {
        foreach (Button button in m_levelButtons)
        {
            if (button == null)
                continue;
            
            button.onClick.RemoveAllListeners();
            Destroy(button.gameObject);
        }
        
        m_levelButtons = Array.Empty<Button>();
    }
    
    protected void Reset()
    {
        ClearLevelButtons();
    }
}
