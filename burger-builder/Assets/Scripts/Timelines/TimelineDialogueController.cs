using System;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class TimelineDialogueController : MonoBehaviour
{
    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private TextMeshProUGUI m_text;

    private bool m_waitingForClick;
    private bool m_skipDialogue;

    private double m_clipEndTime;

    #region Set Up

    private void OnEnable()
    {
        MenuClicking.OnClick += OnClick;
        TimelineManager.OnCutsceneEnd += EndDialogue;
    }

    private void OnDisable()
    {
        MenuClicking.OnClick -= OnClick;
        TimelineManager.OnCutsceneEnd -= EndDialogue;
    }
    
    #endregion

    public void EndDialogue()
    {
        m_waitingForClick = false;
        m_skipDialogue = false;
        m_clipEndTime = 0;
    }

    private void OnClick()
    {
        //first click finishes currently typing line
        if (!m_waitingForClick)
        {
            m_skipDialogue = true;
            return;
        }

        //second click continues dialogue
        m_waitingForClick = false;
        
        //move Timeline to the end of the current dialogue clip
        m_director.time = m_clipEndTime;
        m_director.Evaluate();
        
        m_director.Play();
    }

    public void ClearSkipRequest()
    {
        m_skipDialogue = false;
    }

    public void WaitForClick(Playable playable, bool skipped)
    {
        //indicates that the dialogue is complete and the next click will continue to the next dialogue clip
        m_waitingForClick = true;
        
        //set the time for the end of the clip
        m_clipEndTime = m_director.time + (playable.GetDuration() - playable.GetTime());
        
        //if dialogue has been skipped, don't pause to allow animations to keep playing
        //pause if the end of the dialogue has been reached
        if (!skipped || m_director.time >= m_clipEndTime)
            m_director.Pause();
    }
    
    public TextMeshProUGUI GetText => m_text;
    
    public bool GetSkipDialogue => m_skipDialogue;
}