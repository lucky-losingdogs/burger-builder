using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerParent<T> : MonoBehaviour where T : UnityEngine.Object
{
    protected T[] m_levelDataItems;
    [SerializeField] int m_listExtensionSize = 50;
    
    #region Set Up

    //enable called by game manager to subscribe after game manager instance has been set up
    protected virtual void OnEnable()
    {
        GameManager.OnLevelStart += HandleLevelStart;
    }

    protected virtual void OnDisable()
    {
        GameManager.OnLevelStart -= HandleLevelStart;
    }

    #endregion

    protected virtual void HandleLevelStart(LevelData levelData)
    {
        List<T> rawData = GetRawData(levelData);
        List<T> processedData = ProcessData(rawData, levelData);
        m_levelDataItems = processedData.ToArray();
        SetValues(levelData);
    }
    
    //<summary>
    //override to get data from a specific source
    //e.g: levelData.GetTickets()
    //</summary>
    protected virtual List<T> GetRawData(LevelData levelData)
    {
        return new List<T>();
    }
    
    //extend and shuffle the list of data taken from the level data
    protected virtual List<T> ProcessData(List<T> data, LevelData levelData)
    {
        List<T> tempList = new List<T>(data);
        
        if (m_listExtensionSize > 0)
        {
            tempList = Utilities.ExtendList(tempList, m_listExtensionSize);
        }

        return Utilities.Shuffle(tempList);
    }
    
    //<summary>
    //override to set private array = m_levelDataItems for clarity
    //e.g: m_levelTickets = m_levelDataItems;
    //</summary>
    protected abstract void SetValues (LevelData levelData);
    
    //clear all the stored ticket data
    public virtual void Reset()
    {
        m_levelDataItems = null;
    }
}
