using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float m_spawnRadius = 100f;
    [SerializeField] private float m_fadeDuration = 0.15f;
    
    private Vector2 m_redBarOffset = new Vector2(45, -23);
    private bool m_finishedSliding = false;

    private enum Opacity
    {
        FadeIn = 1,
        FadeOut = 0
    }
    
    private Queue<PerkUI> m_perkQueue = new Queue<PerkUI>();

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
        
        PerkUI perkUI = perk.GetComponent<PerkUI>();
        if (perkUI == null)
            return;
        
        m_perkQueue.Enqueue(perkUI);
        perkUI.SetName(perkData.GetName());

        StartCoroutine(C_DelayedSlide(perkUI.GetBlackBar(), perkUI.GetRedBar()));
        StartCoroutine(C_Lifetime());
    }

    private IEnumerator C_DelayedSlide(GameObject blackBar, GameObject redBar)
    {
        m_finishedSliding = false;
        
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

        if (m_perkQueue.Count > 0)
        {
            yield return new WaitUntil(() => CheckSliding(blackRect, targetPoint, 0.1f));
            yield return new WaitUntil(() => CheckSliding(redRect, redTarget, 0.1f));
            m_finishedSliding = true;
        }
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
        yield return new WaitUntil(() => m_finishedSliding);
        yield return new WaitForSeconds(m_lifetime);
        
        if (m_perkQueue.Count > 0)
        {
            PerkUI currentPerk = m_perkQueue.Dequeue();
            currentPerk.Fade((float)Opacity.FadeOut, m_fadeDuration);
        }

        m_finishedSliding = false;
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
}
