using System;
using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour
{
    [HideInInspector]
    public Transform startPoint;
    [HideInInspector]
    public Transform endPoint;

    SpriteRenderer sr;
    bool hasReachedGoal = false;

    private void Awake()
    {
        startPoint = GetComponentInChildren<StartPoint>(true).transform;
        endPoint = GetComponentInChildren<EndPoint>(true).transform;
        sr = endPoint.GetComponent<SpriteRenderer>();
    }

    internal void Init()
    {
        hasReachedGoal = false ;
        sr.color = Color.white;

        //Debug.DrawRay(startPoint.position, Vector3.right * 3, Color.green, 100);
        //Debug.DrawRay(endPoint.position, -Vector3.right * 3, Color.red, 100);
    }

    internal void GoalReached(Color color)
    {
        if (hasReachedGoal == false)
        {
            StopCoroutine(AnimateColor(Color.clear));
            StartCoroutine(AnimateColor(color));
            hasReachedGoal = true;
        }
    }

    IEnumerator AnimateColor(Color color)
    {
        var starColor = Color.white;
        var endColor = color;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            var currentColor = Color.Lerp(starColor, endColor, t);
            sr.material.SetColor("_Color", currentColor);
            yield return null;
        }
    }

}
