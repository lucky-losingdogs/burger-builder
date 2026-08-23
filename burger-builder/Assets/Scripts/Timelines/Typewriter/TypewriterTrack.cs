using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(TypewriterClip))]
[TrackBindingType(typeof(TimelineDialogueController))]
public class TypewriterTrack : TrackAsset
{
}