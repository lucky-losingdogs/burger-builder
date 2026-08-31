using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] private Canvas m_cutsceneCanvas;
    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private CutsceneData[] m_cutscenes;

    public static event Action<PlayableAsset> OnCutscenePlay;
    public static event Action OnCutsceneEnd;

    private bool m_otherCutscenePlayed = false;

    #region Set Up
    
    private void Awake()
    {
        GetPlayableAssets();
    }

    private void OnEnable()
    {
        LevelManager.OnLevelStart += HandleLevelStart;
        m_director.stopped += HandleCutsceneEnd;
    }
    
    private void OnDisable()
    {
        LevelManager.OnLevelStart -= HandleLevelStart;
        m_director.stopped -= HandleCutsceneEnd;
    }

    private void HandleLevelStart(LevelData levelData)
    {
        Cutscenes cutscene = levelData.GetCutscene();
        if (cutscene != Cutscenes.None)
        {
            PlayCutscene(GetCutscene(cutscene));
            m_otherCutscenePlayed = true;
        }
        else
        {
            PlayStartScene();
        }
    }

    private void GetPlayableAssets()
    {
        List<PlayableData> playableDataList = Utilities.LoadList<PlayableData>("Playables");
        List<CutsceneData> cutsceneList = new List<CutsceneData>();

        foreach (PlayableData playableData in playableDataList)
        {
            cutsceneList.Add(playableData.CutsceneData);
        }
        
        m_cutscenes = cutsceneList.ToArray();
    }
    
    #endregion

    private CutsceneData GetCutscene(Cutscenes cutscene)
    {
        return Array.Find(m_cutscenes,x => x.cutscene == cutscene);
    }
    
    public void PlayCutscene(CutsceneData data)
    {
        if (data == null)
        {
            Debug.LogWarning($"No playable assigned to cutscene: {data.cutscene}");
            return;
        }

        m_cutsceneCanvas.gameObject.SetActive(true);
        GameTime.RequestPause();
        m_director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
        
        m_director.Play(data.playable);
    }

    private void HandleCutsceneEnd(PlayableDirector director)
    {
        m_cutsceneCanvas.gameObject.SetActive(false);
        GameTime.ReleasePause();
        
        if (m_otherCutscenePlayed)
        {
            m_otherCutscenePlayed = false;
            PlayStartScene();
        }
        
        OnCutsceneEnd?.Invoke();
    }

    private void PlayStartScene()
    {
        //Debug.Log($"Play Start Scene: {m_director.playableAsset.name}");
        PlayCutscene(GetCutscene(Cutscenes.RequiredTickets));
    }
}