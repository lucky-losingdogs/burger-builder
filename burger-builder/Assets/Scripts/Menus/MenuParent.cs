using UnityEngine;

public class MenuParent : MonoBehaviour
{
    [SerializeField] protected SceneData m_targetScene;
    private int m_targetSceneIndex;

    protected void Awake()
    {
        //get the scene index of the new scene that can be loaded
        m_targetSceneIndex = m_targetScene.GetIndex();
    }

    //shows and hides what is referenced
    public void TogglePage(GameObject show, GameObject hide)
    {
        show.SetActive(true);
        hide.SetActive(false);
    }

    //quits application
    public void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    //loads a new level
    public void LoadTargetScene()
    {
        if (m_targetSceneIndex >= 0)
        {
            LoadScene.s_instance.StartLoading(m_targetSceneIndex);
            m_targetSceneIndex = (int)NullIndex.NullLevel;
        }
    }
}
