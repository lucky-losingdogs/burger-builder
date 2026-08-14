using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ExitMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private InputActionReference m_anyKeyboard;
    
    public static event Action OnClickOff;

    private bool m_clickingEnabled = false;

    private void OnEnable()
    {
        StartMenu.OnTitleLoaded += EnableClicking;
        
        m_anyKeyboard.action.Enable();
        m_anyKeyboard.action.performed += OnAnyKeyboard;
    }

    private void OnDisable()
    {
        StartMenu.OnTitleLoaded -= EnableClicking;

        DisableKeyboard();
    }

    //when the player clicks on the background invoke event
    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_clickingEnabled)
        {
            OnClickOff?.Invoke();
        }
    }

    //when the player presses any key invoke event
    //only used at start for "Press any button" prompt and then input is disabled
    private void OnAnyKeyboard(InputAction.CallbackContext context)
    {
        if (m_clickingEnabled)
        {
            OnClickOff?.Invoke();
            DisableKeyboard();
        }
    }

    //only enable clicking after text has finished fading in
    private void EnableClicking()
    {
        m_clickingEnabled = true;
    }
    
    private void DisableKeyboard()
    {
        m_anyKeyboard.action.Disable();
        m_anyKeyboard.action.performed -= OnAnyKeyboard;
    }
}
