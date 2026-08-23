using System;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "PlayableData", menuName = "Scriptable Objects/PlayableData")]
public class PlayableData : ScriptableObject
{
    [field: SerializeField] private CutsceneData cutsceneData;
    
    public CutsceneData CutsceneData => cutsceneData;
}

public enum Cutscenes
{
    None, Tutorial, RequiredTickets
}

[Serializable]
public class CutsceneData
{
    public Cutscenes cutscene;
    public PlayableAsset playable;
    public bool hasDialogue;
}