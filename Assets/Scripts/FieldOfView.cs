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

    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        visibleObstacles.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget= (target.position - transform.position).normalized;
            
            // Kontrollera att transform.forward �r faktiskt r�tt
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }

        Collider[] targetsInViewRadius2 = Physics.OverlapSphere(transform.position, viewRadius, obstacleMask);

        //float viewArea = (float)(viewRadius * viewRadius * Math.PI);

        for (int i = 0; i < targetsInViewRadius2.Length; i++)
        {

            Transform target = targetsInViewRadius2[i].transform;
            Vector3 dirToTarget= (target.position - transform.position).normalized;

            // Kontrollera att transform.forward är faktiskt rött
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                for (int k = 0; i < 24; i++)
                {
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(transform.position, transform.forward, out hit, RAY_DIST, obstacleMask))
                    {
                        Debug.DrawRay(transform.position, dirToTarget * RAY_DIST, Color.yellow);
                        visibleObstacles.Add(hit.point);
                    }
                }
             
                /*
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, targetMask))
                {
                    visibleTargets.Add(target);
                }
                */
            }
        }
    }

}
