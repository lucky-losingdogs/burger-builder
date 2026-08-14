using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSpawner : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ShapeData m_shape;
    [SerializeField] private GameObject m_draggablePrefab;

    [SerializeField] private float m_cooldownDuration = 1.5f;
    private bool m_cooling = false;

    private BoardRenderer m_boardRenderer;
    
    private float m_originalCooldownDuration;

    private void Start()
    {
        if (m_draggablePrefab == null)
            m_draggablePrefab = SpawnManager.s_instance.GetDraggablePrefab();

        //get variables from static manager
        m_boardRenderer = SpawnManager.s_instance.GetBoardRenderer();
        
        m_originalCooldownDuration = m_cooldownDuration;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_cooling)
            SpawnItem();
    }

    private void SpawnItem()
    {
        //spawn a draggable item and populate it
        GameObject newDrag = Instantiate(m_draggablePrefab, m_boardRenderer.transform);
        PopulateItem(newDrag, m_boardRenderer);

        m_cooling = true;
        StartCoroutine(C_SpawnCooldown());
    }

    //get the draggable script and pass in variables in initialisation function
    private void PopulateItem(GameObject newDrag, BoardRenderer boardRenderer)
    {
        Item item = newDrag.GetComponent<Item>();
        if (item == null)
            return;

        //set the initial position to be the item spawner
        //z value is specified 0 to not be on the z of ui
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0);
        item.Initialise(m_shape, Utilities.RoundToInt(pos));

        //add to board's list of items and add tiles
        boardRenderer.SetTiles(item);
    }

    //prevent more item spawns until cooldown is completed
    private IEnumerator C_SpawnCooldown()
    {
        yield return new WaitForSeconds(m_cooldownDuration);
        m_cooling = false;
    }
    
    public ShapeData GetShape() => m_shape;
    public float GetOriginalCooldown() => m_originalCooldownDuration;
    public float GetCooldown() => m_cooldownDuration;
    public void SetCooldown(float newCooldown) => m_cooldownDuration = newCooldown;
}
