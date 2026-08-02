using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class PerkManager : ManagerParent<PerkData>
{
    private PerkUIManager m_UIManager;
    private PerkData[] m_levelPerks;

    [SerializeField] private Button debugButton;
    
    #region Set Up

    private void Awake()
    {
        m_UIManager = GetComponent<PerkUIManager>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ComboManager.OnPerkAchieved += HandlePerkAchieved;
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        ComboManager.OnPerkAchieved -= HandlePerkAchieved;
    }

    protected override List<PerkData> GetRawData(LevelData levelData)
    {
        return levelData.GetPerks().ToList();
    }
    
    protected override void HandleLevelStart(LevelData levelData)
    {
        base.HandleLevelStart(levelData);
        SetValues(levelData);
    }

    protected override void SetValues(LevelData levelData)
    {
        m_levelPerks = m_levelDataItems;
    }
    
    #endregion

    //trigger a random perk
    private void HandlePerkAchieved()
    {
        if (m_levelPerks.Length <= 0)
            return;
        
        Random rnd = new Random();
        int index  = rnd.Next(0, m_levelPerks.Length - 1);
        
        PerkData perk = m_levelPerks[index];
        UpdateUI(perk);
        perk.Effect();
    }

    private void UpdateUI(PerkData perk)
    {
        if (m_UIManager == null)
            return;

        m_UIManager.SpawnPerk(perk);
    }

    public void DebugButton()
    {
        HandlePerkAchieved();
    }
}
