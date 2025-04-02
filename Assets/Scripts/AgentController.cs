using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField]
    CharacterController controller;
    [SerializeField]
    private InfluenceMap InfluenceMap;
    [SerializeField]
    private FieldOfView fieldOfView;

    [SerializeField]
    private float movementSpeed;

    Vector3 goal;
    Vector3 dir;
    Vector3 dest;

    void Start()
    {
        dest = new Vector3((int)transform.position.x, 0, (int)transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Add agents position to influence map
        InfluenceMap.AddPoint(new Vector2(transform.position.x, transform.position.z), 0.0f);

        // If distance between agent and destinations is less than 1.0: find new point or else move towards destination
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), dest) < 1.0)
        {
            InfluenceMap.FindPoint();
            dest = InfluenceMap.goToPos;
            dest.y = 0;
            dir = dest - new Vector3(transform.position.x, 0.0f, transform.position.z);
            dir.Normalize();
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            controller.Move(dir * movementSpeed * Time.deltaTime);
        }

        fieldOfView.FindVisibleTargets();


        // Loop all visible osbtaclepoints and add to IM
        foreach (Vector3 obstacle in fieldOfView.visibleObstacles)
        {
            InfluenceMap.AddPoint(new Vector2(obstacle.x, obstacle.z), 0.2f);
        }

        // Loop all visible targets (player) and add to IM, set as destination and rotate agent
        foreach (Transform target in fieldOfView.visibleTargets)
        {
            InfluenceMap.AddPoint(new Vector2(target.position.x, target.position.z), 1.0f);
            dest = new Vector3(target.position.x, 0, target.position.z);
            dir = dest - new Vector3(transform.position.x, 0.0f, transform.position.z);
            dir.Normalize();
            transform.rotation = Quaternion.LookRotation(dir);
        }

    }
}
