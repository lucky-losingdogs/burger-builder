using UnityEngine;
using UnityEngine.Playables;

public class TypewriterBehaviour : PlayableBehaviour
{
    public string m_text;
    public float m_tolerance = 1f;

    private bool m_finished = false;
    private bool m_skipped = false;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        m_finished = false;
        m_skipped = false;
    }
    
    public override void ProcessFrame(Playable playable, FrameData info, object obj)
    {
        //dont process anything unless this clip is actually playing.
        if (info.effectivePlayState != PlayState.Playing)
            return;
        
        TimelineDialogueController controller = obj as TimelineDialogueController;
        if (controller == null)
            return;
        
        
        float time = (float)playable.GetTime(); //get the local time of the clip
        float duration = (float)playable.GetDuration(); //get duration of clip
        float progress = Mathf.Clamp01((time / duration) / (1f - m_tolerance));
        
        int characterCount; //the number of characters that will be revealed

        //if player has clicked to skip dialogue/has previously done so
        //set character count to the full length of the text to display the whole thing
        if (controller.GetSkipDialogue || m_skipped)
        {
            controller.ClearSkipRequest();
            characterCount = m_text.Length;
            m_skipped = true;
        }
        else //only display the next few characters like a typewriter
        {
            characterCount = Mathf.FloorToInt(progress * m_text.Length);
        }
        
        //set text in controller
        controller.GetText.text = m_text;
        controller.GetText.maxVisibleCharacters = characterCount;

        //if the whole line has been displayed, wait for click to progress dialogue
        if (characterCount >= m_text.Length && !m_finished)
        {
            m_finished = true;
            controller.WaitForClick(playable, m_skipped);
        }
    }
}