using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager s_instance;

    [SerializeField] private GameObject m_draggablePrefab;
    [SerializeField] private BoardRenderer m_boardRenderer;

    private void Awake()
    {
        //destroy previous singleton
        if (s_instance != null)
        {
            Destroy(s_instance.gameObject);
        }
        s_instance = this;

        //if the managers aren't serialized, find it in scene
        if (m_boardRenderer == null)
            m_boardRenderer = FindFirstObjectByType<BoardRenderer>();
    }

    #region Getters

    public GameObject GetDraggablePrefab()
    {
        return m_draggablePrefab;
    }

    public BoardRenderer GetBoardRenderer()
    {
        return m_boardRenderer;
    }

    #endregion
}