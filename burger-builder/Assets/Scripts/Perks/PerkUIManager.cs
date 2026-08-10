using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PerkUIManager : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject m_perkUIPrefab;
    [SerializeField] private RectTransform[] m_waypoints; //first is spawn point and second is slide end point

    [Header("Values")]
    [SerializeField] private float m_slideDuration = 0.3f;
    [SerializeField] private float m_delayDuration = 0.1f;
    [SerializeField] private float m_lifetime = 1f;
    [SerializeField] private float m_fadeDuration = 0.15f;
    [SerializeField] private float m_spawnRadius = 100f;
    [SerializeField] private Vector2 m_randomRotationRange = new Vector2(-12, 18);
    
    private Vector2 m_redBarOffset = new Vector2(45, -23);
    
    private List<Coroutine> m_lifeTimeCoroutines = new List<Coroutine>();
    private Queue<PerkUI> m_perkQueue = new Queue<PerkUI>();

    private enum Opacity
    {
        FadeIn = 1,
        FadeOut = 0
    }

    private void Awake()
    {
        HideWaypoints();
    }

    public void SpawnPerk(PerkData perkData)
    {
        if (m_waypoints.Length <= 0)
            return;
        
        GameObject perk = Instantiate(m_perkUIPrefab, m_waypoints[0]);
        if (perk == null)
            return;
        
        //add a random float to the z rotation
        perk.transform.eulerAngles += new Vector3(0, 0, Random.Range(m_randomRotationRange.x, m_randomRotationRange.y));
        
        PerkUI perkUI = perk.GetComponent<PerkUI>();
        if (perkUI == null)
            return;
        
        m_perkQueue.Enqueue(perkUI);
        perkUI.SetName(perkData.GetName());
        StartCoroutine(C_DelayedSlide(perkUI, perkUI.GetBlackBar(), perkUI.GetRedBar()));
        
        Coroutine lifetimeCoroutine = StartCoroutine(C_Lifetime());
        m_lifeTimeCoroutines.Add(lifetimeCoroutine);
    }

    private IEnumerator C_DelayedSlide(PerkUI ui, GameObject blackBar, GameObject redBar)
    {
        ui.SetSliding(true);
        
        RectTransform blackRect = blackBar.GetComponent<RectTransform>();
        RectTransform redRect = redBar.GetComponent<RectTransform>();
        if (blackRect == null || redRect == null)
            yield break;

        //get a random point inside a circle and add to the waypoint to get a random point around waypoint
        Vector2 randomPoint = Random.insideUnitCircle * m_spawnRadius;
        Vector2 targetPoint = Utilities.Add(m_waypoints[1].position,  randomPoint);
        
        StartCoroutine(C_Slide(blackRect, m_waypoints[0].position, targetPoint));
        
        //wait a delay between moving bars for effect
        yield return new WaitForSeconds(m_delayDuration);

        Vector2 redTarget = Utilities.Add(targetPoint, m_redBarOffset);
        StartCoroutine(C_Slide(redRect, m_waypoints[0].position, redTarget));

        yield return new WaitUntil(() => CheckSliding(blackRect, targetPoint, 0.1f));
        yield return new WaitUntil(() => CheckSliding(redRect, redTarget, 0.1f));
        ui.SetSliding(false);
    }

    //lerp position of the transform to target over slide duration
    private IEnumerator C_Slide(RectTransform barRect, Vector2 startPos, Vector2 endPos)
    {
        float elapsedTime = 0f;

        while (elapsedTime < m_slideDuration)
        {
            elapsedTime += Time.deltaTime;
            barRect.position = Vector3.Lerp(startPos, endPos, elapsedTime / m_slideDuration);
            yield return null;
        }

        barRect.position = endPos;
    }

    private IEnumerator C_Lifetime()
    {
        if (m_perkQueue.Count > 0)
        {
            PerkUI currentPerk = m_perkQueue.Dequeue();
            if (currentPerk != null)
            {
                yield return new WaitWhile(() => currentPerk.GetSliding());
                yield return new WaitForSeconds(m_lifetime);
                
                currentPerk.Fade((float)Opacity.FadeOut, m_fadeDuration);
                currentPerk.Destroy();
            }
        }
    }

    private bool CheckSliding(RectTransform barRect, Vector2 targetPos, float tolerance)
    {
        float distance = Vector2.Distance(barRect.position, targetPos);
        return distance <= tolerance;
    }

    //set the images of the waypoints to clear so invisible in game
    private void HideWaypoints()
    {
        for (int i = 0; i < m_waypoints.Length; i++)
        {
            Image img = m_waypoints[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = Color.clear;
            }
        }
    }

    public void ClearAllPerks()
    {
        //stop any lifetime coroutines
        foreach (Coroutine coroutine in m_lifeTimeCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        foreach (PerkUI perkUI in m_perkQueue)
        {
            if (perkUI != null)
                perkUI.Destroy();
        }
        
        m_lifeTimeCoroutines.Clear();
        m_perkQueue.Clear();
    }
}
