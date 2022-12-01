using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField]
    CharacterController controller;
    [SerializeField]
    private InfluenceMap InfluenceMap;

    [SerializeField]
    private float movementSpeed;

    Vector3 goal;

    void Start()
    {
        goal = InfluenceMap.goToPos;
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if (goal != null)
        {
            if (seeker.IsDone()) seeker.StartPath(rb.position, goal, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        float distanceToTarget = Vector2.Distance(rb.position, goal.transform.position);
        float speed = unit.speed;

        FlipTransform(direction);

        // If the unit is supposed to flee from player and "kite", negate direction and slow speed
        if (unit.kitingSpeed > 0 && distanceToTarget < unit.range / 2)
        {
            direction = -direction;
            speed = unit.kitingSpeed;
        }

        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        // If the player is within range of the enemy, it will attack
        if (distanceToTarget < unit.range) unit.Attack(goal);


        if (distance < nextWaypointDistance) currentWaypoint++;

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }




    //private Vector3 moveStep = Vector3.zero;

    // Start is called before the first frame update
    /*
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dest = InfluenceMap.goToPos;
        //print("dest: " + dest);
        //print("pos: " + transform.position);
        if (transform.position != dest)
        {
            Vector3 dir = dest - new Vector3(transform.position.x, 0.0f, transform.position.z);
            dir.Normalize();
            controller.Move(dir * movementSpeed * Time.deltaTime);
        }
    }
    */
}
