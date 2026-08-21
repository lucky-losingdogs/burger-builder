using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelSelect : LevelSelectInScene
{
    [SerializeField] private GameObject m_targetLevelPrefab;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartMenu.OnContinueButtonPressed += LoadLevelOutOfScene;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StartMenu.OnContinueButtonPressed -= LoadLevelOutOfScene;
    }
    
    //spawn a DontDestroyOnLoad object carrying the level index
    //then load the main scene where the object passes the level index to the game manager
    protected override void OnButtonClick(int index)
    {
        LoadLevelOutOfScene(index);
    }

    private void LoadLevelOutOfScene(int index)
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
}
