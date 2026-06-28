using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_timerUI;
    [SerializeField] private float m_timeDecrease = 1.0f;

    private string m_template = "Time: ";
    private Coroutine m_timer;
    private float m_timeLimit = 0.0f;

    #region Set Up

    private void Awake()
    {
        if (m_timerUI == null)
            m_timerUI = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameManager.OnLevelStart += SetTimer;
        GameManager.OnLevelEnd += StopTimer;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStart -= SetTimer;
        GameManager.OnLevelEnd -= StopTimer;
    }

    #endregion

    private void SetTimer(LevelData data)
    {
        float limit = data.GetTimeLimit();

        m_timeLimit = limit;

        UpdateUI(m_timeLimit);
        StartTimer();
    }

    public void StartTimer()
    {
        StopTimer();
        m_timer = StartCoroutine(C_Countdown());
    }

    private void StopTimer()
    {
        if (m_timer != null)
            StopCoroutine(m_timer);
    }

    private void UpdateUI(float currentTime)
    {
        m_timerUI.text = m_template + currentTime;
    }

    private IEnumerator C_Countdown()
    {
        float elapsedTime = m_timeLimit;
        
        while (elapsedTime > 0)
        {
            yield return new WaitForSeconds(m_timeDecrease);

            elapsedTime -= m_timeDecrease;
            UpdateUI(elapsedTime);
        }
        Debug.Log("time over");
        GameUIManager.s_instance.ToggleFailMenu(true);
    }
}
