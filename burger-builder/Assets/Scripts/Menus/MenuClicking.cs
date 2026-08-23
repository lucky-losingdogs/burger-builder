using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MenuClicking : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private InputActionReference m_anyKeyboard;
    [SerializeField] private bool m_keepKeyboardActive = false;
    
    public static event Action OnClick;

    private static bool m_clickingEnabled = false;

    private void OnEnable()
    {
        if (m_anyKeyboard == null)
            return;
        m_anyKeyboard.action.Enable();
        m_anyKeyboard.action.performed += OnAnyKeyboard;
    }

    private void OnDisable()
    {
        DisableKeyboard();
    }

    //when the player clicks on the background invoke event
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click");
        if (m_clickingEnabled)
        {
            OnClick?.Invoke();
        }
    }

    //when the player presses any key invoke event
    //only used at start for "Press any button" prompt and then input is disabled
    private void OnAnyKeyboard(InputAction.CallbackContext context)
    {
        if (m_clickingEnabled)
        {
            OnClick?.Invoke();
            if (!m_keepKeyboardActive)
                DisableKeyboard();
        }
    }

    //only enable clicking after text has finished fading in
    public static void EnableClicking()
    {
        m_clickingEnabled = true;
        Debug.Log("Clicking enabled"+m_clickingEnabled);
    }
    
    private void DisableKeyboard()
    {
        if (m_anyKeyboard == null)
            return;
        m_anyKeyboard.action.Disable();
        m_anyKeyboard.action.performed -= OnAnyKeyboard;
    }
}
