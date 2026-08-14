using UnityEngine;
using UnityEngine.SceneManagement;

//<summary>
// pass the target level from the main menu's level select between scenes
//</summary>
public class TargetLevel : MonoBehaviour
{
    private int m_targetLevelIndex = 0;

    #region Set Up

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    #endregion

    //when the scene is loaded, pass the selected level from level select to the GameManager
    //GameManager loads the selected level
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Passing targetLevelIndex: " + m_targetLevelIndex);
        GameManager.s_instance.SetSelectedLevel(m_targetLevelIndex);
        
        //destroy this object afterwards
        Destroy(gameObject);
    }
    
    public void SetTargetLevelIndex(int index) => m_targetLevelIndex = index;
    public int GetTargetLevelIndex() => m_targetLevelIndex;
}
