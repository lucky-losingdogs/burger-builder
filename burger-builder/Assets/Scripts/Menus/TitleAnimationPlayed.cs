using UnityEngine;

public class TitleAnimationPlayed : MonoBehaviour
{
    public static TitleAnimationPlayed s_instance;
    private bool m_titlePlayed = false;

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
    
    public void SetTitlePlayed(bool value) => m_titlePlayed = value;
    public bool GetTitlePlayed() => m_titlePlayed;
}
