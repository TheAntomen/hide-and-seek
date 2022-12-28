using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private float viewRadius;
    [SerializeField]
    private float viewAngle;
    [SerializeField]
    private LayerMask targetMask;
    [SerializeField]
    private LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform> ();
    public List<Vector3> visibleObstacles = new List<Vector3> ();

    private const int RAY_DIST = 100;
    private int rayCount = 50;
    private float angle = 0f;
    private float angleIncrease;
    private float startingAngle;


    private void Start()
    {
        angle = startingAngle;
        angleIncrease = viewAngle / rayCount;
    }

    // Method for finding visible targets and filling respective List, visibleTargets or visibleObstacles
    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        visibleObstacles.Clear();

        angle = GetAngleFromVector(transform.forward) - viewAngle / 2f;

        // Send raycasts from different angles with increment "angleIncrease" for every cast
        for (int i = 0; i <= rayCount; i++)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, GetVectorFromAngle(angle +(i*angleIncrease)), out hit, viewRadius, targetMask);

            // Draw debugging ray for visualizing rays
            Debug.DrawRay(transform.position, GetVectorFromAngle(angle + (i * angleIncrease)) * Vector3.Distance(transform.position, hit.point), Color.yellow);

            // Check if and what the ray hit and add to lists
            if (hit.collider == null)
            {
                // Nothing hit
            }
            else if (hit.collider.gameObject.tag == "Cube")
            {
                visibleObstacles.Add(hit.point);
            }
            else if (hit.collider.gameObject.tag == "Player")
            {
                visibleTargets.Add(hit.transform);
            }
        }
    }

    //------ Mathematical help-functions ------ //

    public static float GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);

        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

}
