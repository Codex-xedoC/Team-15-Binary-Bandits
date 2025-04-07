using System.Collections.Generic;
using UnityEngine;

public class ArrowTargeting : MonoBehaviour
{
    [Header("Arrows")]
    public Transform enemyArrow;   // Assign in Inspector
    public Transform planetArrow;  // Assign in Inspector

    [Header("Arrow Behavior")]
    public float rotationSpeed = 5f;  // Smooth rotation speed

    void Update()
    {
        // Find nearest enemy and planet
        Transform nearestEnemy = GetNearestTarget(GameObject.FindGameObjectsWithTag("Enemy"), enemyArrow.position);
        Transform nearestPlanet = GetNearestTarget(GameObject.FindGameObjectsWithTag("Planet"), planetArrow.position);

        // Point arrows toward their targets
        PointArrowAt(enemyArrow, nearestEnemy);
        PointArrowAt(planetArrow, nearestPlanet);
    }

    Transform GetNearestTarget(GameObject[] targets, Vector3 origin)
    {
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            float dist = Vector3.Distance(origin, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = obj.transform;
            }
        }

        return nearest;
    }

    void PointArrowAt(Transform arrow, Transform target)
    {
        if (arrow == null || target == null) return;

        Vector3 direction = (target.position - arrow.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        arrow.rotation = Quaternion.Slerp(arrow.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}