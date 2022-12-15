using JetBrains.Annotations;
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
    private FieldOfView fieldOfView;

    [SerializeField]
    private float movementSpeed;

    Vector3 goal;
    Vector3 start;
    
    Vector3 move;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 40f);
            controller.Move(dir * movementSpeed * Time.deltaTime);

            //Debug.Log(dest);
        }

        fieldOfView.FindVisibleTargets();

        foreach (Transform target in fieldOfView.visibleTargets)
        {
            InfluenceMap.AddPoint(new Vector2(target.position.x, target.position.z), 1.0f);
        }

        foreach (Vector3 obstacle in fieldOfView.visibleObstacles)
        {
            Debug.Log("Hej?");
            InfluenceMap.AddPoint(new Vector2(obstacle.x, obstacle.z), 0.0f);
        }


        /*
        goal = InfluenceMap.goToPos;
        goal = new Vector3(goal.x, 0, goal.z);
        start = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 dir = movementCalc();
        dir.Normalize();
        controller.Move(dir * movementSpeed * Time.deltaTime);
        */
    }

    Vector3 movementCalc() 
    {
        Color goal_color = InfluenceMap.influenceMap.GetPixel((int)goal.x + 10, (int)goal.z + 10);
        Vector3 goal_vector = (goal - start) * goal_color.r;

        move = new Vector3(0f, 0f, 0f);
        foreach (Vector3 neighbour in getNeighbours(start))
        {
            Color neighbour_color = InfluenceMap.influenceMap.GetPixel((int)neighbour.x + 10, (int)neighbour.z + 10);
            Vector3 neighbours_to_agent_vector = (start - neighbour) * (1-neighbour_color.r);
            
            move += neighbours_to_agent_vector;
        }

        if (goal_color.r > 0.5)
        {
            move += goal_vector;
        }

        print("move after " + move);
        return move;
    }

    List<Vector3> getNeighbours(Vector3 pos)
    {
        List<Vector3> neighbours = new List<Vector3>();

        //top
        neighbours.Add(new Vector3(pos.x, 0, pos.z + 1));
        //bot
        neighbours.Add(new Vector3(pos.x, 0, pos.z - 1));
        //right
        neighbours.Add(new Vector3(pos.x + 1, 0, pos.z));
        //left
        neighbours.Add(new Vector3(pos.x - 1, 0, pos.z));
        //northeast
        neighbours.Add(new Vector3(pos.x + 1, 0, pos.z + 1));
        //northwest
        neighbours.Add(new Vector3(pos.x - 1, 0, pos.z + 1));
        //southeast
        neighbours.Add(new Vector3(pos.x + 1, 0, pos.z - 1));
        //southwest
        neighbours.Add(new Vector3(pos.x - 1, 0, pos.z - 1));
        
        return neighbours;
    }

}
