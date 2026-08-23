using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TypewriterClip : PlayableAsset
{
    [TextArea(3, 10)]
    public string text;
    
    public float tolerance = 1f;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        ScriptPlayable<TypewriterBehaviour> playable = ScriptPlayable<TypewriterBehaviour>.Create(graph);

        TypewriterBehaviour behaviour = playable.GetBehaviour();

        behaviour.m_text = text;
        behaviour.m_tolerance = tolerance;

        return playable;
    }
}