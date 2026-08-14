using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : ManagerParent<ShapeData>
{
    public static SpawnManager s_instance;

    [SerializeField] private GameObject m_draggablePrefab;
    [SerializeField] private BoardRenderer m_boardRenderer;
    [SerializeField] private List<GameObject> m_spawnerObjects;

    private ShapeData[] m_levelShapes;
    private Dictionary<ShapeData, GameObject> m_spawnerDictionary = new Dictionary<ShapeData, GameObject>();
    private List<ItemSpawner> m_activeSpawners = new List<ItemSpawner>();
    
    private List<float> m_spawnerCooldownModifiers = new List<float>();

    #region Set Up
    
    private void Awake()
    {
        //destroy previous singleton
        if (s_instance != null)
        {
            Destroy(s_instance.gameObject);
        }
        s_instance = this;

        if (m_boardRenderer == null)
            m_boardRenderer = FindFirstObjectByType<BoardRenderer>();

        SetDictionary();
        HideSpawners();
    }

    #region  Level Data
    
    protected override List<ShapeData> GetRawData(LevelData levelData)
    {
        return levelData.GetShapes().ToList();
    }
    
    protected override void HandleLevelStart(LevelData levelData)
    {
        List<ShapeData> rawData = GetRawData(levelData);
        m_levelDataItems = rawData.ToArray();
        SetValues(levelData);
        SetLevelSpawners();
    }

    protected override void SetValues(LevelData levelData)
    {
        m_levelShapes = m_levelDataItems;
    }

    #endregion
    
    private void SetDictionary()
    {
        if (m_spawnerObjects.Count == 0)
            return;
        
        foreach (GameObject spawnerObject in m_spawnerObjects)
        {
            ItemSpawner itemSpawner = spawnerObject.GetComponent<ItemSpawner>();
            if (itemSpawner == null)
                continue;

            ShapeData shape = itemSpawner.GetShape();
            if (shape == null)
                continue;
            
            m_spawnerDictionary.Add(shape, spawnerObject);
            m_activeSpawners.Add(itemSpawner);
        }
    }

    private void HideSpawners()
    {
        foreach (GameObject spawnerObject in m_spawnerObjects)
        {
            spawnerObject.SetActive(false);
        }
    }

    private void SetLevelSpawners()
    {
        foreach (ShapeData shape in m_levelShapes)
        {
            foreach (ShapeData spawnerShape in m_spawnerDictionary.Keys)
            {
                if (spawnerShape == shape)
                    m_spawnerDictionary[spawnerShape].SetActive(true);
            }
        }
    }

    #endregion
    
    #region Perk Behaviour
    
    public void ChangeCooldown(float duration, float value)
    {
        StartCoroutine(PerkManager.C_EffectDuration(AddCooldownModifier, RemoveCooldownModifier, duration, value));
    }
    
    private void AddCooldownModifier(float value)
    {
        m_spawnerCooldownModifiers.Add(value);
        RecalculateCooldown();
    }

    private void RemoveCooldownModifier(float value)
    {
        m_spawnerCooldownModifiers.Remove(value);
        RecalculateCooldown();
    }

    private void RecalculateCooldown()
    {
        foreach (ItemSpawner itemSpawner in m_activeSpawners)
        {
            float newCooldown = PerkManager.Recalculate(m_spawnerCooldownModifiers, itemSpawner.GetCooldown());
            itemSpawner.SetCooldown(newCooldown);
        }
    }
    
    #endregion
    
    #region Getters

    public GameObject GetDraggablePrefab()
    {
        return m_draggablePrefab;
    }

    public BoardRenderer GetBoardRenderer()
    {
        return m_boardRenderer;
    }

    #endregion
}