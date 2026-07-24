using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class KeyboardManager : MonoBehaviour
{
    [SerializeField] private InputActionReference m_previousInput;
    [SerializeField] private InputActionReference m_nextInput;

    public static event Action OnPreviousInput;
    public static event Action OnNextInput;

    #region Set Up

    private void Awake()
    {
        if (m_previousInput == null)
            Debug.LogError("Previous Input is null");
        if (m_nextInput == null)
            Debug.LogError("Next Input is null");
    }

    private void OnEnable()
    {
        m_previousInput.action.Enable();
        m_previousInput.action.performed += HandlePreviousInput;

        m_nextInput.action.Enable();
        m_nextInput.action.performed += HandleNextInput;
    }

    private void OnDisable()
    {
        m_previousInput.action.Disable();
        m_previousInput.action.performed -= HandlePreviousInput;

        m_nextInput.action.Disable();
        m_nextInput.action.performed -= HandleNextInput;
    }

    #endregion

    private void HandlePreviousInput(InputAction.CallbackContext context)
    {
        OnPreviousInput.Invoke();
    }

    private void HandleNextInput(InputAction.CallbackContext context)
    {
        OnNextInput.Invoke();
    }
}
