using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager s_instance;

    private SaveData saveData;
    
    public static event Action<SaveData> OnSaveLoaded;

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);
        SaveSystem.DeleteSave();
        LoadSave();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        
        LevelManager.OnLevelEnd += HandleNextLevel;
        LevelManager.OnLevelStart += HandleLevelStart;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        
        LevelManager.OnLevelEnd -= HandleNextLevel;
        LevelManager.OnLevelStart -= HandleLevelStart;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PassSave());
    }

    private void HandleLevelStart(LevelData levelData)
    {
        StartCoroutine(PassSave());
    }

    private IEnumerator PassSave()
    {
        yield return null;
        OnSaveLoaded?.Invoke(saveData);
    }

    private void LoadSave()
    {
        SaveData save = SaveSystem.LoadGame();
        saveData = save;
    }

    private void HandleNextLevel()
    {
        int currentLevel = LevelManager.s_instance.GetCurrentLevel();
        if (currentLevel > saveData.highestLevel)
            SaveGame(SaveSystem.UpdateSavedLevel(saveData, currentLevel));
    }
    
    private void SaveGame(SaveData save)
    {
        SaveSystem.SaveGame(save);
        LoadSave();
    }
    
    public SaveData GetSaveData() => saveData;
}

public interface ILoadableSaveData
{
    public void HandleSaveLoaded(SaveData saveData);

    public List<LevelData> GetList(int highestLevel)
    {
        List<LevelData> allLevels = Utilities.LoadList<LevelData>("Levels");
        allLevels = Utilities.TrimExcess(allLevels, highestLevel + 1);
        return allLevels;
    }
}