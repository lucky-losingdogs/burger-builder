using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Logic;
using UnityEngine;

public class PerkManager : ManagerParent<PerkData>
{
    private GameContext m_context;
    
    private PerkUIManager m_UIManager;
    private PerkData[] m_levelPerks;
    
    #region Set Up

    private void Awake()
    {
        m_UIManager = GetComponent<PerkUIManager>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ComboManager.OnPerkAchieved += HandlePerkAchieved;
        GameManager.OnContextCreated += SetGameContext;
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        ComboManager.OnPerkAchieved -= HandlePerkAchieved;
        GameManager.OnContextCreated -= SetGameContext;
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

    private void SetGameContext(GameContext context)
    {
        m_context = context;
        if (m_context == null)
            Debug.LogError("GameContext is null!");
    }
    
    #endregion

    //trigger a random perk
    private void HandlePerkAchieved(float combo)
    {
        if (m_levelPerks.Length <= 0)
            return;
        
        List<PerkData> rankPerks = GetRankPerks(combo);
        if (rankPerks.Count == 0)
            return;
        
        int index  = Random.Range(0, rankPerks.Count - 1);
        
        PerkData perk = rankPerks[index];
        
        UpdateUI(perk);
        ActivatePerk(perk);
    }

    //trigger the effect of the perk by getting the class w/ logic from data dictionary
    private void ActivatePerk(PerkData perk)
    {
        PerkTypes perkType = perk.GetPerkType();
        if (Data.Perks.TryGetValue(perkType, out PerkLogic logic))
            logic.Effect(m_context);
    }

    private List<PerkData> GetRankPerks(float combo)
    {
        List<PerkData> rankPerks = new List<PerkData>();

        foreach (PerkData data in m_levelPerks)
        {
            float rank = data.GetRank();
            Debug.Log("combo" + combo);
            Debug.Log("rank"+rank);
            if (combo >= rank)
            {
                rankPerks.Add(data);
            }
        }

        return rankPerks;
    }

    private void UpdateUI(PerkData perk)
    {
        if (m_UIManager == null)
            return;

        m_UIManager.SpawnPerk(perk);
    }
}
