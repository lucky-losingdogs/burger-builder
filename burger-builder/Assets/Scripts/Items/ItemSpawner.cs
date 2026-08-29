using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSpawner : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ShapeData m_shape;
    [SerializeField] private GameObject m_draggablePrefab;

    [SerializeField] private float m_cooldownDuration = 1.5f;
    private bool m_cooling = false;
    private float m_originalCooldownDuration;

    private BoardRenderer m_boardRenderer;
    private DragManager m_dragManager;

    [SerializeField] private int m_itemLimit = 6;
    private List<Item> m_draggableItems = new List<Item>();

    private void Start()
    {
        if (m_draggablePrefab == null)
            m_draggablePrefab = SpawnManager.s_instance.GetDraggablePrefab();

        //get variables from static manager
        m_boardRenderer = SpawnManager.s_instance.GetBoardRenderer();
        m_dragManager = SpawnManager.s_instance.GetDragManager();
        
        m_originalCooldownDuration = m_cooldownDuration;

        SpawnItemPool();
    }

    private void SpawnItemPool()
    {
        for (int i = 1; i < m_itemLimit; i++)
        {
            Item newItem = SpawnItem();
            if (newItem == null)
                return;
            
            m_draggableItems.Add(newItem);
            newItem.gameObject.SetActive(false);
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;
        
        Item currentItem = null;
        if (!m_cooling)
            currentItem = ShowItem();

        if (currentItem != null)
            m_dragManager.BeginDraggingItem(eventData, currentItem);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        m_dragManager.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_dragManager.OnEndDrag(eventData);
    }

    private Item SpawnItem()
    {
        //spawn a draggable item and populate it
        GameObject newDrag = Instantiate(m_draggablePrefab, m_boardRenderer.transform);
        Item newItem = PopulateItem(newDrag, m_boardRenderer);
        return newItem;
    }

    //get the draggable script and pass in variables in initialisation function
    private Item PopulateItem(GameObject newDrag, BoardRenderer boardRenderer)
    {
        Item item = newDrag.GetComponent<Item>();
        if (item == null)
            return item;

        SetItemPosition(item);

        return item;
    }

    private void SetItemPosition(Item item)
    {
        //set the initial position to be the item spawner
        //z value is specified 0 to not be on the z of ui
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0);
        item.Initialise(m_shape, Utilities.RoundToInt(pos));
    }

    private Item ShowItem()
    {
        Item currentItem = null;
        
        foreach (Item item in m_draggableItems)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
                currentItem = item;
                //reset the item pos each time its 'spawned'/shown
                SetItemPosition(item);
                
                m_boardRenderer.SetTiles(item);
                
                m_cooling = true;
                StartCoroutine(C_SpawnCooldown());
                break;
            }
        }

        return currentItem;
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
