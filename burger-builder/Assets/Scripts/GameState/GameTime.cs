using UnityEngine;

//<summary>
//control timeScale in one place
//to prevent overlapping timeScale changes
//</summary>
public static class GameTime
{
    private static int m_pauseRequests;

    //called to pause time
    public static void RequestPause()
    {
        m_pauseRequests++;
        UpdateTimeScale();
    }

    //decrease the num of pause requests
    //if it hits 0 unpause time
    public static void ReleasePause()
    {
        m_pauseRequests = Mathf.Max(0, m_pauseRequests - 1);
        UpdateTimeScale();
    }

    //update time scale based on if there is a request to pause time
    private static void UpdateTimeScale()
    {
        if (m_pauseRequests > 0)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
}