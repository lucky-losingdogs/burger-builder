using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public static LoadScene s_instance;
    
    private int m_targetSceneIndex = (int)NullIndex.NullLevel;

    [SerializeField] private GameObject m_loadingUI;

    #region Set Up

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);

        m_loadingUI.SetActive(false);
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

    public void StartLoading(int index)
    {
        m_loadingUI.SetActive(true);
        GameTime.ReleasePause();

        m_targetSceneIndex = index;
        if (m_targetSceneIndex == SceneManager.GetActiveScene().buildIndex)
        {
            ResetIndex();
            return;
        }

        StartCoroutine(C_LoadAsync(m_targetSceneIndex));
    }

    protected void UnloadScene(int index)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(index);
        if (!scene.isLoaded)
            return;

        //async unload the scene at the passed in index
        if (index >= 0 && index != SceneManager.GetActiveScene().buildIndex)
            SceneManager.UnloadSceneAsync(index);

        m_loadingUI.SetActive(false);
    }

    protected IEnumerator C_LoadAsync(int index)
    {
        //ensure time isn't paused
        GameTime.ReleasePause();

        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        loadAsync.allowSceneActivation = false;

        while (!loadAsync.isDone)
        {
            if (loadAsync.progress >= 0.9f)
            {
                loadAsync.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //set active scene as the loaded scene
        SceneManager.SetActiveScene(scene);

        //unload all other scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene tempScene = SceneManager.GetSceneAt(i);

            if (tempScene != scene && tempScene.isLoaded)
            {
                UnloadScene(tempScene.buildIndex);
            }
        }
    }

    private void ResetIndex()
    {
        m_targetSceneIndex = (int)NullIndex.NullLevel;
    }
}


public enum NullIndex { NullLevel = -1 }