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
    Vector3 move;

    bool bajskorv = true;
    void Start()
    {
        //InfluenceMap.FindPoint();
        //dest = InfluenceMap.goToPos;
        dest = new Vector3((int)transform.position.x, 0, (int)transform.position.z);
        //dir = dest - new Vector3(transform.position.x, 0.0f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
        InfluenceMap.AddPoint(new Vector2(transform.position.x, transform.position.z), 0.0f);

        //InfluenceMap.FindPoint();
        //print("dest: " + dest);
        //print(new Vector3((int)transform.position.x, 0, (int)transform.position.z));
        //Debug.Log(Vector3.Distance(new Vector3((int)transform.position.x, 0, (int)transform.position.z), dest));
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), dest) < 1.0)
        {
            Debug.Log("NEW POS!!!");
            InfluenceMap.FindPoint();
            dest = InfluenceMap.goToPos;
            dest.y = 0;
            print("dest: " + dest);
            dir = dest - new Vector3(transform.position.x, 0.0f, transform.position.z);
            dir.Normalize();
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            controller.Move(dir * movementSpeed * Time.deltaTime);
            print(new Vector3((int)transform.position.x, 0, (int)transform.position.z));
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
        }

        fieldOfView.FindVisibleTargets();

        foreach (Transform target in fieldOfView.visibleTargets)
        {
            InfluenceMap.AddPoint(new Vector2(target.position.x, target.position.z), 1.0f);
            dest = new Vector3(target.position.x, 0, target.position.z);
            dir = dest - new Vector3(transform.position.x, 0.0f, transform.position.z);
            dir.Normalize();
            transform.rotation = Quaternion.LookRotation(dir);
        }

        foreach (Vector3 obstacle in fieldOfView.visibleObstacles)
        {
            InfluenceMap.AddPoint(new Vector2(obstacle.x, obstacle.z), 0.2f);
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

    Vector3 movementCalc(Vector3 goalDir) 
    {
        Color goal_color = InfluenceMap.influenceMap.GetPixel((int)goal.x + 20, (int)goal.z + 20);
        //Vector3 goal_vector = (goal - start) * goal_color.r;

        Vector3 start = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 move = new Vector3(0, 0, 0);
        foreach (Vector3 neighbour in getNeighbours(start))
        {
            Color neighbour_color = InfluenceMap.influenceMap.GetPixel((int)neighbour.x + 20, (int)neighbour.z + 20);
            Debug.Log("Color: " + neighbour_color);
            
            if (neighbour_color.r < 0.5)
            {
                Vector3 neighbours_to_agent_vector = (start - neighbour) * (1 - neighbour_color.r);
                move += neighbours_to_agent_vector;
            }
        }

        
        if (goal_color.r > 0.5)
        {
            move += goalDir;
        }
        
        

        print("move after " + move);
        return move.normalized;
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

        //top2
        //neighbours.Add(new Vector3(pos.x, 0, pos.z + 2));
        //bot2
      //  neighbours.Add(new Vector3(pos.x, 0, pos.z - 2));
        //right2
      //  neighbours.Add(new Vector3(pos.x + 2, 0, pos.z));
        //left2
      //  neighbours.Add(new Vector3(pos.x - 2, 0, pos.z));
        //northeast2
       // neighbours.Add(new Vector3(pos.x + 2, 0, pos.z + 2));
        //northwest2
      //  neighbours.Add(new Vector3(pos.x - 2, 0, pos.z + 2));
        //southeast2
     //   neighbours.Add(new Vector3(pos.x + 2, 0, pos.z - 2));
        //southwest2
      //  neighbours.Add(new Vector3(pos.x - 2, 0, pos.z - 2));

        return neighbours;
    }

}
